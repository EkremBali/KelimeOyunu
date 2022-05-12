using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tablo : MonoBehaviour
{
    //Random harf yerle�tirmek i�in ingiliz alfabesi.
    char[] alphabet = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

    //Harfleri ta��yan k�plerin transform dizisi.
    public Transform[] matris;

    //Kelime Listesi.
    public List<Kelime> wordList;

    //S�ras� ile �nce liste s�ralan�r daha sonra tabloya yerle�tirilir ard�ndan random harfler yerle�ir son olarak ise yerle�meyen kelimeler kontrol edilir.
    void Start()
    {
        sortByLength();
        wayControl();
        randomLetter();
        control();
    }

    //Kelime listesinin kelime uzunlu�una g�re s�ralanmas�n� sa�layan metod.
    public void sortByLength()
    {
        wordList.Sort(delegate (Kelime x, Kelime y)
        {
            if (x.getIng() == y.getIng()) return 0;
            else if (x.getIng().Length > y.getIng().Length) return -1;
            else return 1;

        });
    }

    //Kelimelerin way'ini kontrol edip yerle�tirme metodunu �a��ran metod.
    public void wayControl()
    {
        foreach (var word in wordList)
        {
            switch (word.getWay())
            {
                case "saso":
                    placement(word, 0, -1);
                    break;
                case "sosa":
                    placement(word, 0, 1);
                    break;
                case "ya":
                    placement(word, 1, 0);
                    break;
                case "ay":
                    placement(word, -1, 0);
                    break;
                case "soysaa":
                    placement(word, 1, 1);
                    break;
                case "saysoa":
                    placement(word, 1, -1);
                    break;
                case "soasay":
                    placement(word, -1, 1);
                    break;
                case "saasoy":
                    placement(word, -1, -1);
                    break;
                default:
                    break;

            }
        }
    }

    //Ba�lang��ta kendisine gelen sat�r-s�tun de�i�imleri ile kelimeyi yerle�tirmeye �al���r, kelime yerle�mez ise di�er way'ler denenir.
    public void placement(Kelime word, int rowChange, int columnChange)
    {
        string k = word.getIng();
        char[] wordChar = k.ToCharArray();

        //Kelime yerle�ti mi?
        bool isDone = true;

        //D�ng� bitti mi?
        bool isNotFinish = true;

        //Kelimeyi yerle�tirmek i�in ilk seferde random sat�r-s�tun de�i�kenleri ve bunlar�n ilk hallerinin saklanmas�.
        int row = Random.Range(0, 11);    int prevRow = row;
        int column = Random.Range(0, 8);     int prevColumn = column;

        // Daha sonra yerle�tirmenin ba�tan m� sondan m� ba�layaca��n� tutan random de�i�ken.
        int placementRandom = Random.Range(0, 2);

        //Her sat�r ve s�tun'un denenmesi i�in tutulan ba�lang�� ve son de�erleri.
        int newRowStart = 0; int newColumnStart = -1;
        int newRowEnd = 10; int newColumnEnd = 8;

        //D�ng�n�n toplam d�nece�i hak de�i�keni ve revize edilen sat�r-s�tun de�i�im de�erlerini tutacak dizi.
        int hak = 8;   int[] rowCol = new int[2];

        do
        {
            //Bu d�ng�de mevcut way i�in b�t�n sat�r ve s�tunlar denenir.Yerle�emez ise d�ng� sonlan�r ve yeni bir way belirlenir.
            while (isNotFinish)
            {
                //Kelime uzunlu�unca d�nen, kullan�lacak k�plerin sat�r-s�tun de�i�kenleri ile kontrol�n� yapan for d�ng�s�.
                for (int i = 0; i < k.Length; i++)
                {
                    if (column == 8 || column == -1 || row == 11 || row == -1)
                    {
                        isDone = false;
                        break;
                    }
                    if (!matris[row].GetChild(column).CompareTag("kullanildi"))
                    {
                        row += rowChange;
                        column += columnChange;
                    }
                    else
                    {
                        isDone = false;
                        break;
                    }
                }

                //Kelimenin yerle�ece�i yolda s�k�n�t� ��kmad� ise burada yerle�tirilir. Her bir harfin hangi k�pte oldu�u points dizisinde tutulur.
                //Son olarak t�m d�ng�lerin sonlanaca�� de�i�kenler atan�r.
                if (isDone)
                {
                    int[,] points = new int[k.Length, 2];

                    for (int i = 0; i < k.Length; i++)
                    {
                        matris[prevRow].GetChild(prevColumn).GetChild(0).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "" + wordChar[i];
                        matris[prevRow].GetChild(prevColumn).tag = "kullanildi";

                        points[i, 0] = prevRow;
                        points[i, 1] = prevColumn;

                        prevRow += rowChange;
                        prevColumn += columnChange;
                        
                    }
                    word.isSettle = true;
                    word.setPoints(points);

                    hak = -1;
                    isNotFinish = false;
                }
                else
                {
                    //placementRandom de�i�keni 0 ise yerle�tirme denemelerine ba�tan aksi halde sondan ba�lan�r.
                    if (placementRandom == 0)
                    {
                        newColumnStart++;
                        if (newColumnStart == 8)
                        {
                            newColumnStart = 0;
                            newRowStart++;
                            if (newRowStart == 11)
                            {
                                word.isSettle = false;
                                isNotFinish = false;
                                newRowStart = 0;
                                newColumnStart = -1;
                            }
                        }
                        row = newRowStart;
                        column = newColumnStart;
                    }
                    else
                    {
                        newColumnEnd--;
                        if (newColumnEnd == -1)
                        {
                            newColumnEnd = 7;
                            newRowEnd--;
                            if (newRowEnd == -1)
                            {
                                word.isSettle = false;
                                isNotFinish = false;
                                newRowEnd = 10;
                                newColumnEnd = 8;
                            }
                        }
                        row = newRowEnd;
                        column = newColumnEnd;
                    }

                    prevRow = row;
                    prevColumn = column;
                    
                }
                isDone = true;
            }

            //Hak d���r�l�r, hak bitmedi ise yeni way ve sat�r-s�tun de�i�imleri ayarlan�r ve isNotFinish true yap�larak yukar�daki d�ng� ile yeniden yerle�tirme denenir.
            hak--;
            if(hak > 0)
            {
                rowCol = rowColRevised(word);
                rowChange = rowCol[0];
                columnChange = rowCol[1];
                isNotFinish = true;
            }

        } while (hak > 0);
        

    }

    //Gelen kelime i�in yeni way'i belirler, ard�ndan sat�r-s�tun de�i�imlerini bir dizide geri d�ner.
    public int[] rowColRevised(Kelime word)
    {
        int[] rowCol = new int[2];

        word.setNextWay();

        switch (word.getWay())
        {
            case "saso":
                rowCol[0] = 0;
                rowCol[1] = -1;
                break;
            case "sosa":
                rowCol[0] = 0;
                rowCol[1] = 1;
                break;
            case "ya":
                rowCol[0] = 1;
                rowCol[1] = 0;
                break;
            case "ay":
                rowCol[0] = -1;
                rowCol[1] = 0;
                break;
            case "soysaa":
                rowCol[0] = 1;
                rowCol[1] = 1;
                break;
            case "saysoa":
                rowCol[0] = 1;
                rowCol[1] = -1;
                break;
            case "soasay":
                rowCol[0] = -1;
                rowCol[1] = 1;
                break;
            case "saasoy":
                rowCol[0] = -1;
                rowCol[1] = -1;
                break;
            default:
                break;

        }

        return rowCol;
    }

    //Kelime i�in kullan�lmam�� k�plere random harf yaz�lmas�n� sa�layan metod.
    public void randomLetter()
    {
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (!matris[i].GetChild(j).CompareTag("kullanildi"))
                {
                    matris[i].GetChild(j).GetChild(0).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = alphabet[Random.Range(0, 26)].ToString();
                }
            }

        }

    }

    //Her kelime i�in yerle�me durumunu kontrol eden metod.
    public void control()
    {
        foreach(var word in wordList)
        {
            word.control();
        }
    }

    //Tabloya yerle�tire metodlar�n�n s�ras� ile �a��r�lams�n� sa�layan metod.
    public void callInOrder()
    {
        //Bu d�ng� ile her bir k�p'�n tagleri, renkleri s�f�rlan�p SetActive'leri true olarak ayarlan�r.
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                matris[i].GetChild(j).tag = "KUP";
                matris[i].GetChild(j).GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(185f / 255f, 185f / 255f, 185f / 255f, 255f / 255f);
                matris[i].GetChild(j).gameObject.SetActive(true);
            }
        }

        sortByLength();
        wayControl();
        randomLetter();
        control();
    }


}
