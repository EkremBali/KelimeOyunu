using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class script : MonoBehaviour
{
    //Dokunulan ilk son ve bir önceki dokunulmuþ son küpler.
    Transform first, last, prevLast;

    //Çizilen, denenen, düþen elemanlarý tutan listeler.
    List<Transform> drawedElements;
    List<Transform> triedElements;
    List<Transform> downElements;

    //Sesler
    public AudioSource touchSound;
    public AudioSource hint1, hint2, hint3;
    public AudioSource soundFound, soundUnfound;

    //Müzik kapama butonu.
    public GameObject soundButton;

    //Kelime Listesi
    public List<Kelime> wordList;

    //Harfleri taþýyan küplerin transform dizisi.
    public Transform[] matris;

    //Küpler için kullanýlan z pozisyonlarý
    public float[] zPositions = { 3.5f, 2.5f, 1.5f, .5f, -.5f, -1.5f, -2.5f, -3.5f };

    //Ýlk dokunuþ mu?
    bool isBegan = true;

    //Ýpucu aktifleþti mi?
    bool isHintActive = false;

    //Kullanýlan renkler.
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

            //Began fazý ipucu için kullanýlýyor.
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

            //Dokunuþ hareket ederse ve herhangi bir küpteki sphere'e temas olur ise isBegan deðiþkenine göre hareket boyuncaki ilk ve son temas edilen sphereler tutulur.
            //Ardýndan temas edilen küplerin çizimi için draw metodu çaðrýlýr.
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
                //Dokunuþ sonlandýðýndan isBegan yine true yapýlýr.
                isBegan = true;

                //Çizilen elemanlar tek tek geziliyor.TextMeshlerine ulaþýlýyor ve harfler word deðiþkeninde toplanýyor.Böylece kelime alýnmýþ oluyor.
                string word = "";
                Transform text;
                foreach (var element in drawedElements)
                {
                    text = element.GetChild(0).GetChild(0).GetChild(0);
                    word += text.gameObject.GetComponent<TextMeshProUGUI>().text;
                }

                //Kelime listesi geziliyor eðer ki çizilen kelime bir kelimeye eþit ise eleman bulunmuþ oluyor.
                foreach (var k in wordList)
                {
                    if (k.getIng() == word)
                    {
                        //Her bir harf için küpler geziliyor.
                        foreach (var element in drawedElements)
                        {
                            element.gameObject.AddComponent<Rigidbody>();

                            //patalama efekti sýkýþmasýn diye küp y ekseninde 1 birim yükseltiliyor.
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
                //Bu durumda kelime eþleþmemiþ demektir.
                if (drawedElements.Count != 0)
                {
                    foreach (var element in drawedElements)
                    {
                        element.GetChild(0).GetChild(0).GetComponent<Image>().color = gray;
                    }
                    soundUnfound.Play();
                    drawedElements.Clear();

                    //Eðerki çizilmiþ bir ipucu var ise kelime bulunmadýðý takdirde tekrar çizilmesi gerekir.
                    drawHint();
                }
            }
        }
    }

    //Temas edilen küplerin çizilmesini saðlayan metod.
    public void draw()
    {
        //Çizilmiþ elemanlar yine çizilir ki elimiz yoldan çýkarsa çizim kaybolmasýn.
        //foreach (var element in drawedElements)
        //{
        //    element.GetChild(0).GetChild(0).GetComponent<Image>().color = blue;
        //}

        //first ve last elemanýn, satýr-sütun noktalarýný tutan diziler.
        int[] firstPoint = new int[2];
        int[] lastPoint = new int[2];

        //Bütün küpler geziliyor, buradan first ve last küplerin satýr-sütunlarý bulunuyor.
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

        //Çizime baþlamak için baþlangýç noktasý olarak firstPoint in elemanlarý row ve column'a atanýyor.
        int row = firstPoint[0]; int column = firstPoint[1];

        //setChange metodu ile first ve last elemanlar arasýndaki satýr-sütun deðiþimleri belirleniyor.
        int rowChange = setChange(lastPoint[0] , firstPoint[0]);
        int columnChange = setChange(lastPoint[1], firstPoint[1]);

        //Bu döngü boyunca first elemandan last elemana deðiþim verileri ile ilerleyerek çizim yapýlmaya çalýþýlýyor. Eðer last eleman çizim yapýlamayacak
        //bir doðrultuda ise bulunamýyor böylece denenen elemanlar temizlenip döngüden çýkýlýyor.Bulunur ise denenen elemanlar çizilip çizilen elemanlara aktarýlýyor.
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

    //Ýpucu aktif deðil ise butona bastýðýmýzda kelime panellerini ve dokunmak için küplerini aktif eden metod.
    //Ýpucu aktif iken çalýþmaz ki bir ipucu seçilmeden butona tekrar basýlarak hak kaybedilmesin.
    public void choseHint()
    {
        if (!isHintActive)
        {
            int hCount = PlayerPrefs.GetInt("hintCount");

            //Ýpucu hakký 0 deðilse paneller açýlýr, gerekli ses çalýnýr ve hak azaltýlýr. Aksi halde sadece ses çalar.
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

    //Ýpucu aktif hale gitirilmiþ harflerin yeþil renge boyanmasýný saðlayan metod.
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

    //Yeni bölüme geçince listeleri sýfýrlayan metod.
    public void nextLevelDelete()
    {
        drawedElements.Clear();
        triedElements.Clear();
    }

    //Düþen küplerin tag, renk ve transformlarýnýn sýfýrlanýp görünmez hale gelmesini saðlayan metod.
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


