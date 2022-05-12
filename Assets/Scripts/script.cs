using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class script : MonoBehaviour
{
    //Dokunulan ilk son ve bir �nceki dokunulmu� son k�pler.
    Transform first, last, prevLast;

    //�izilen, denenen, d��en elemanlar� tutan listeler.
    List<Transform> drawedElements;
    List<Transform> triedElements;
    List<Transform> downElements;

    //Sesler
    public AudioSource touchSound;
    public AudioSource hint1, hint2, hint3;
    public AudioSource soundFound, soundUnfound;

    //M�zik kapama butonu.
    public GameObject soundButton;

    //Kelime Listesi
    public List<Kelime> wordList;

    //Harfleri ta��yan k�plerin transform dizisi.
    public Transform[] matris;

    //K�pler i�in kullan�lan z pozisyonlar�
    public float[] zPositions = { 3.5f, 2.5f, 1.5f, .5f, -.5f, -1.5f, -2.5f, -3.5f };

    //�lk dokunu� mu?
    bool isBegan = true;

    //�pucu aktifle�ti mi?
    bool isHintActive = false;

    //Kullan�lan renkler.
    Color gray = new Color(185f / 255f, 185f / 255f, 185f / 255f, 255f / 255f);
    Color blue = new Color(14f / 255f, 128f / 255f, 125f / 255f, 255f / 255f);
    Color green = new Color(103f / 255f, 219f / 255f, 104f / 255f, 255f / 255f);

    void Start()
    {
        drawedElements = new List<Transform>();
        triedElements = new List<Transform>();
        downElements = new List<Transform>();
        PlayerPrefs.SetInt("hintCount", 5);
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            //Began faz� ipucu i�in kullan�l�yor.
            if(touch.phase == TouchPhase.Began)
            {
                if (Physics.Raycast(ray, out hit, 100f))
                {
                    if (hit.transform.CompareTag("hintCube") && hit.transform.parent.GetComponent<Kelime>().isFind != true && hit.transform.GetChild(0).GetComponent<Image>().enabled)
                    {
                        hit.transform.parent.GetComponent<Kelime>().isHintActive = true;
                        hit.transform.parent.GetComponent<Kelime>().nextHint();

                        drawHint();
                        hint3.Play();

                        foreach (var word in wordList)
                        {
                            if(word.transform.childCount == 3)
                            {
                                word.transform.GetChild(2).GetChild(0).GetComponent<Image>().enabled = false;
                            }
                            else if(word.transform.childCount == 4)
                            {
                                word.transform.GetChild(3).GetChild(0).GetComponent<Image>().enabled = false;
                            }
                        }
                        isHintActive = false;

                    }
                }
            }

            //Dokunu� hareket ederse ve herhangi bir k�pteki sphere'e temas olur ise isBegan de�i�kenine g�re hareket boyuncaki ilk ve son temas edilen sphereler tutulur.
            //Ard�ndan temas edilen k�plerin �izimi i�in draw metodu �a�r�l�r.
            if (touch.phase == TouchPhase.Moved)
            {
                if (Physics.Raycast(ray, out hit, 100f))
                {
                    if (hit.transform.CompareTag("SPHERE"))
                    {
                        if (isBegan)
                        {
                            first = hit.transform.parent;
                            isBegan = false;
                        }
                        prevLast = last;
                        last = hit.transform.parent;
                        draw();
                    }
                }
            }

            else if (touch.phase == TouchPhase.Ended)
            {
                //Dokunu� sonland���ndan isBegan yine true yap�l�r.
                isBegan = true;

                //�izilen elemanlar tek tek geziliyor.TextMeshlerine ula��l�yor ve harfler word de�i�keninde toplan�yor.B�ylece kelime al�nm�� oluyor.
                string word = "";
                Transform text;
                foreach (var element in drawedElements)
                {
                    text = element.GetChild(0).GetChild(0).GetChild(0);
                    word += text.gameObject.GetComponent<TextMeshProUGUI>().text;
                }

                //Kelime listesi geziliyor e�er ki �izilen kelime bir kelimeye e�it ise eleman bulunmu� oluyor.
                foreach (var k in wordList)
                {
                    if (k.getIng() == word)
                    {
                        //Her bir harf i�in k�pler geziliyor.
                        foreach (var element in drawedElements)
                        {
                            element.gameObject.AddComponent<Rigidbody>();

                            //patalama efekti s�k��mas�n diye k�p y ekseninde 1 birim y�kseltiliyor.
                            float z = element.localPosition.z;
                            float x = element.localPosition.x;
                            element.localPosition = new Vector3(x, 1, z);

                            element.gameObject.GetComponent<Animation>().Play();
                            downElements.Add(element);
                        }

                        soundFound.Play();
                        Invoke("destroyDowned", 3);
                        k.found();
                        drawedElements.Clear();
                    }
                }
                //Bu durumda kelime e�le�memi� demektir.
                if (drawedElements.Count != 0)
                {
                    foreach (var element in drawedElements)
                    {
                        element.GetChild(0).GetChild(0).GetComponent<Image>().color = gray;
                    }
                    soundUnfound.Play();
                    drawedElements.Clear();

                    //E�erki �izilmi� bir ipucu var ise kelime bulunmad��� takdirde tekrar �izilmesi gerekir.
                    drawHint();
                }
            }
        }
    }

    //Temas edilen k�plerin �izilmesini sa�layan metod.
    public void draw()
    {
        //�izilmi� elemanlar yine �izilir ki elimiz yoldan ��karsa �izim kaybolmas�n.
        //foreach (var element in drawedElements)
        //{
        //    element.GetChild(0).GetChild(0).GetComponent<Image>().color = blue;
        //}

        //first ve last eleman�n, sat�r-s�tun noktalar�n� tutan diziler.
        int[] firstPoint = new int[2];
        int[] lastPoint = new int[2];

        //B�t�n k�pler geziliyor, buradan first ve last k�plerin sat�r-s�tunlar� bulunuyor.
        for (int i=0; i<11; i++)
        {
            for(int j=0; j<8; j++)
            {
                if(first == matris[i].GetChild(j))
                {
                    firstPoint[0] = i;
                    firstPoint[1] = j;
                }
                
                if (last == matris[i].GetChild(j))
                {
                    lastPoint[0] = i;
                    lastPoint[1] = j;
                }
            }
        }

        //�izime ba�lamak i�in ba�lang�� noktas� olarak firstPoint in elemanlar� row ve column'a atan�yor.
        int row = firstPoint[0]; int column = firstPoint[1];

        //setChange metodu ile first ve last elemanlar aras�ndaki sat�r-s�tun de�i�imleri belirleniyor.
        int rowChange = setChange(lastPoint[0] , firstPoint[0]);
        int columnChange = setChange(lastPoint[1], firstPoint[1]);

        //Bu d�ng� boyunca first elemandan last elemana de�i�im verileri ile ilerleyerek �izim yap�lmaya �al���l�yor. E�er last eleman �izim yap�lamayacak
        //bir do�rultuda ise bulunam�yor b�ylece denenen elemanlar temizlenip d�ng�den ��k�l�yor.Bulunur ise denenen elemanlar �izilip �izilen elemanlara aktar�l�yor.
        for (int i = 0; i < 11; i++)
        {
            if (column == 8 || column == -1 || row == 11 || row == -1)
            {
                triedElements.Clear();
                break;
            }
            else if(matris[row].GetChild(column) != last)
            {
                triedElements.Add(matris[row].GetChild(column));
                row += rowChange;
                column += columnChange;     
            }
            else
            {
                if (last != prevLast)
                {
                    touchSound.Play();
                }

                foreach (var element in drawedElements)
                {
                    element.GetChild(0).GetChild(0).GetComponent<Image>().color = gray;
                }
                drawedElements.Clear();


                triedElements.Add(matris[row].GetChild(column));
                foreach (var element in triedElements)
                {
                    drawedElements.Add(element);
                    element.GetChild(0).GetChild(0).GetComponent<Image>().color = blue;
                }
                triedElements.Clear();
                break;
            }
        }

    }

    //�pucu aktif de�il ise butona bast���m�zda kelime panellerini ve dokunmak i�in k�plerini aktif eden metod.
    //�pucu aktif iken �al��maz ki bir ipucu se�ilmeden butona tekrar bas�larak hak kaybedilmesin.
    public void choseHint()
    {
        if (!isHintActive)
        {
            int hCount = PlayerPrefs.GetInt("hintCount");

            //�pucu hakk� 0 de�ilse paneller a��l�r, gerekli ses �al�n�r ve hak azalt�l�r. Aksi halde sadece ses �alar.
            if (hCount != 0)
            {
                hint1.Play();

                foreach (var word in wordList)
                {
                    if (word.transform.childCount == 3)
                    {
                        word.transform.GetChild(2).GetChild(0).GetComponent<Image>().enabled = true;
                    }
                    else if (word.transform.childCount == 4)
                    {
                        word.transform.GetChild(3).GetChild(0).GetComponent<Image>().enabled = true;
                    }
                }

                hCount--;
                PlayerPrefs.SetInt("hintCount", hCount);
                isHintActive = true;
            }
            else
            {
                hint2.Play();
            }
        }
    }

    //�pucu aktif hale gitirilmi� harflerin ye�il renge boyanmas�n� sa�layan metod.
    public void drawHint()
    {
        int[,] points;
        bool[] hintActive;

        foreach(var word in wordList)
        {
            if (word.isHintActive)
            {
                points = word.getPoints();
                hintActive = word.getHintActive();

                for (int i = 0; i<hintActive.Length; i++)
                {
                    if(hintActive[i] == true)
                    {
                        int satir = points[i, 0];
                        int sutun = points[i, 1];
                        matris[satir].GetChild(sutun).GetChild(0).GetChild(0).GetComponent<Image>().color = green;
                    }
                }
            }
        }
    }

    //Yeni b�l�me ge�ince listeleri s�f�rlayan metod.
    public void nextLevelDelete()
    {
        drawedElements.Clear();
        triedElements.Clear();
    }

    //D��en k�plerin tag, renk ve transformlar�n�n s�f�rlan�p g�r�nmez hale gelmesini sa�layan metod.
    public void destroyDowned()
    {
        foreach (var eleman in downElements)
        {
            Destroy(eleman.GetComponent<Rigidbody>());
            eleman.GetChild(0).GetChild(0).GetComponent<Image>().color = gray;

            float z  = 0;
            Transform p = eleman.parent;
            for(int i=0; i<8; i++)
            {
                if(p.GetChild(i) == eleman)
                {
                    z = zPositions[i];
                }
            }
            eleman.localPosition = new Vector3(0, 0, z);
            eleman.localRotation = new Quaternion(0,0,0,0);
            eleman.localScale = new Vector3(1, 1, 1);

            eleman.gameObject.SetActive(false);
        }
        downElements.Clear();
    }

    public void soundToggle()
    {
        if (GetComponent<AudioSource>().mute)
        {
            GetComponent<AudioSource>().mute = false;
            soundButton.transform.GetChild(1).gameObject.SetActive(true);
            soundButton.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            GetComponent<AudioSource>().mute = true;
            soundButton.transform.GetChild(1).gameObject.SetActive(false);
            soundButton.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    public int setChange(int son, int bas)
    {
        if (son > bas) return 1;
        else if (son < bas) return -1;
        else return 0;
    }

}


