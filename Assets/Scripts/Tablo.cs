using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tablo : MonoBehaviour
{
    //Random harf yerleþtirmek için ingiliz alfabesi.
    char[] alphabet = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

    //Harfleri taþýyan küplerin transform dizisi.
    public Transform[] matris;

    //Kelime Listesi.
    public List<Kelime> wordList;

    //Sýrasý ile önce liste sýralanýr daha sonra tabloya yerleþtirilir ardýndan random harfler yerleþir son olarak ise yerleþmeyen kelimeler kontrol edilir.
    void Start()
    {
        sortByLength();
        wayControl();
        randomLetter();
        control();
    }

    //Kelime listesinin kelime uzunluðuna göre sýralanmasýný saðlayan metod.
    public void sortByLength()
    {
        wordList.Sort(delegate (Kelime x, Kelime y)
        {
            if (x.getIng() == y.getIng()) return 0;
            else if (x.getIng().Length > y.getIng().Length) return -1;
            else return 1;

        });
    }

    //Kelimelerin way'ini kontrol edip yerleþtirme metodunu çaðýran metod.
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

    //Baþlangýçta kendisine gelen satýr-sütun deðiþimleri ile kelimeyi yerleþtirmeye çalýþýr, kelime yerleþmez ise diðer way'ler denenir.
    public void placement(Kelime word, int rowChange, int columnChange)
    {
        string k = word.getIng();
        char[] wordChar = k.ToCharArray();

        //Kelime yerleþti mi?
        bool isDone = true;

        //Döngü bitti mi?
        bool isNotFinish = true;

        //Kelimeyi yerleþtirmek için ilk seferde random satýr-sütun deðiþkenleri ve bunlarýn ilk hallerinin saklanmasý.
        int row = Random.Range(0, 11);    int prevRow = row;
        int column = Random.Range(0, 8);     int prevColumn = column;

        // Daha sonra yerleþtirmenin baþtan mý sondan mý baþlayacaðýný tutan random deðiþken.
        int placementRandom = Random.Range(0, 2);

        //Her satýr ve sütun'un denenmesi için tutulan baþlangýç ve son deðerleri.
        int newRowStart = 0; int newColumnStart = -1;
        int newRowEnd = 10; int newColumnEnd = 8;

        //Döngünün toplam döneceði hak deðiþkeni ve revize edilen satýr-sütun deðiþim deðerlerini tutacak dizi.
        int hak = 8;   int[] rowCol = new int[2];

        do
        {
            //Bu döngüde mevcut way için bütün satýr ve sütunlar denenir.Yerleþemez ise döngü sonlanýr ve yeni bir way belirlenir.
            while (isNotFinish)
            {
                //Kelime uzunluðunca dönen, kullanýlacak küplerin satýr-sütun deðiþkenleri ile kontrolünü yapan for döngüsü.
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

                //Kelimenin yerleþeceði yolda sýkýnýtý çýkmadý ise burada yerleþtirilir. Her bir harfin hangi küpte olduðu points dizisinde tutulur.
                //Son olarak tüm döngülerin sonlanacaðý deðiþkenler atanýr.
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
                    //placementRandom deðiþkeni 0 ise yerleþtirme denemelerine baþtan aksi halde sondan baþlanýr.
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

            //Hak düþürülür, hak bitmedi ise yeni way ve satýr-sütun deðiþimleri ayarlanýr ve isNotFinish true yapýlarak yukarýdaki döngü ile yeniden yerleþtirme denenir.
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

    //Gelen kelime için yeni way'i belirler, ardýndan satýr-sütun deðiþimlerini bir dizide geri döner.
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

    //Kelime için kullanýlmamýþ küplere random harf yazýlmasýný saðlayan metod.
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

    //Her kelime için yerleþme durumunu kontrol eden metod.
    public void control()
    {
        foreach(var word in wordList)
        {
            word.control();
        }
    }

    //Tabloya yerleþtire metodlarýnýn sýrasý ile çaðýrýlamsýný saðlayan metod.
    public void callInOrder()
    {
        //Bu döngü ile her bir küp'ün tagleri, renkleri sýfýrlanýp SetActive'leri true olarak ayarlanýr.
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
