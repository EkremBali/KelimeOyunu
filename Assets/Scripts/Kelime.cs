using UnityEngine;
using TMPro;

public class Kelime: MonoBehaviour
{
    //Kelimenin ana yap�s�.
    public TextMeshProUGUI wordFromUI;
    public string tr;
    public string ing;
    public string way;
    public bool isFind = false;

    //Kelime tabloya yerle�ti mi?
    public bool isSettle;

    //�pucu sistemi i�in kullan�lan yap�lar.
    int[,] points;
    public bool[] hintActive;
    public bool isHintActive = false;

    //Kullan�lacak yerle�me y�ntemleri ve ba�lang�� i�in random de�eri.
    string[] ways = { "sosa", "saso", "ya", "ay", "soysaa", "saysoa", "soasay", "saasoy" };   
    int random = 0;

    void Start()
    {
        //points kelimenin tabloda yerle�ti�i k�plerin sat�r ve s�tununu tutan iki boyutlu dizi, hint active ise her bir harf i�in 
        //ipucu al�n�p al�nmad���n� tutan bool dizi, ba�lang�� i�in hepsi false olarak atan�d�.
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

    //Kelime i�in random way olu�turan metod.
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

    //Bir kelimenin yerle�ebilmesi i�in b�t�n yollar�n denenmesini sa�layan, random de�erine g�re s�ras� ile ya ileri ya geri giderek way de�i�tiren metod.
    //Her �a��r�ld���nda bir �nceki-sonraki way'i alm�� olur.
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

    //Kelime bulundu�unda text rengini de�i�tiren, TextMesh eleman�n�n olu�ma durumunu kontrol ederek sar� y�ld�z� a�an ve isFind de�i�kenini true yapan metod.
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

    //Yeni level'a ge�ildi�inde kelimenin s�f�rlanmas�n� sa�layan metod.
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

    //Kelime tabloya yerle�emedi ise textini bo�altan metod.
    public void control()
    {
        if (!isSettle)
        {
            wordFromUI.text = "-";
        }
    }

    //Her ipucu �a��r�ld���nda kelimenin s�radaki harfi i�in dizideki eleman� true yapan metod.
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
