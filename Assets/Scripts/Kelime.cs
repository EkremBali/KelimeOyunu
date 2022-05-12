using UnityEngine;
using TMPro;

public class Kelime: MonoBehaviour
{
    //Kelimenin ana yapýsý.
    public TextMeshProUGUI wordFromUI;
    public string tr;
    public string ing;
    public string way;
    public bool isFind = false;

    //Kelime tabloya yerleþti mi?
    public bool isSettle;

    //Ýpucu sistemi için kullanýlan yapýlar.
    int[,] points;
    public bool[] hintActive;
    public bool isHintActive = false;

    //Kullanýlacak yerleþme yöntemleri ve baþlangýç için random deðeri.
    string[] ways = { "sosa", "saso", "ya", "ay", "soysaa", "saysoa", "soasay", "saasoy" };   
    int random = 0;

    void Start()
    {
        //points kelimenin tabloda yerleþtiði küplerin satýr ve sütununu tutan iki boyutlu dizi, hint active ise her bir harf için 
        //ipucu alýnýp alýnmadýðýný tutan bool dizi, baþlangýç için hepsi false olarak atanýdý.
        points = new int[ing.Length, 2];
        hintActive = new bool[ing.Length];
        clearHintActive();

        random = Random.Range(0, 2);

        createWay();
    }

    public string getIng()
    {
        return ing;
    }
    public void setIng(string ing)
    {
        this.ing = ing;
    }

    public string getTr()
    {
        return tr;
    }
    public void setTr(string tr)
    {
        this.tr = tr;
        wordFromUI.text = tr;
    }

    public string getWay()
    {
        return way;
    }
    public void setWay(string way)
    {
        this.way = way;
    }

    public int[,] getPoints()
    {
        return points;
    }
    public void setPoints(int[,] points)
    {
        this.points = points;
    }

    public bool[] getHintActive()
    {
        return hintActive;
    }

    //Kelime için random way oluþturan metod.
    public void createWay()
    {
        if (ing.Length > 8)
        {
            way = ways[Random.Range(2, 4)];
        }
        else
        {
            way = ways[Random.Range(0, 8)];
        }

    }

    //Bir kelimenin yerleþebilmesi için bütün yollarýn denenmesini saðlayan, random deðerine göre sýrasý ile ya ileri ya geri giderek way deðiþtiren metod.
    //Her çaðýrýldýðýnda bir önceki-sonraki way'i almýþ olur.
    public void setNextWay()
    {
        for(int i=0; i < ways.Length; i++)
        {
            if (random == 0)
            {
                if (way == ways[i])
                {
                    if (i == ways.Length - 1)
                    {
                        way = ways[0];
                        break;
                    }
                    else
                    {
                        way = ways[i + 1];
                        break;
                    }

                }
            }
            else
            {
                if (way == ways[i])
                {
                    if (i == 0)
                    {
                        way = ways[ways.Length-1];
                        break;
                    }
                    else
                    {
                        way = ways[i - 1];
                        break;
                    }

                }
            }
            
        }   
    }

    //Kelime bulunduðunda text rengini deðiþtiren, TextMesh elemanýnýn oluþma durumunu kontrol ederek sarý yýldýzý açan ve isFind deðiþkenini true yapan metod.
    public void found()
    {
        wordFromUI.color = new Color(0f / 255f, 197f / 255f, 212f / 255f, 255f / 255f);
        if(transform.childCount == 3)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else if(transform.childCount == 4)
        {
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
        }
        
        isFind = true;
    }

    //Yeni level'a geçildiðinde kelimenin sýfýrlanmasýný saðlayan metod.
    public void unfound()
    {
        random = Random.Range(0, 2);

        wordFromUI.color = new Color(63f / 255f, 23f / 255f, 8f / 255f, 255f / 255f);
        if (transform.childCount == 3)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else if (transform.childCount == 4)
        {
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(false);
        }

        isFind = false;
        isSettle = false;
        isHintActive = false;

        createWay();

        points = new int[ing.Length, 2];
        hintActive = new bool[ing.Length];
        clearHintActive();
    }

    //Kelime tabloya yerleþemedi ise textini boþaltan metod.
    public void control()
    {
        if (!isSettle)
        {
            wordFromUI.text = "-";
        }
    }

    //Her ipucu çaðýrýldýðýnda kelimenin sýradaki harfi için dizideki elemaný true yapan metod.
    public void nextHint()
    {
        for(int i=0; i<ing.Length; i++)
        {
            if(hintActive[i] == false)
            {
                hintActive[i] = true;
                break;
            }
        }
    }

    //ipucu dizisini temizleyen metod.
    public void clearHintActive()
    {
        for(int i = 0; i<ing.Length; i++)
        {
            hintActive[i] = false;
        }
    }

}
