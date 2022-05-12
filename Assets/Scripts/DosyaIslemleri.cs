using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class wordsModel
{
    public string[] word1;
    public string[] word2;
    public string[] word3;
    public string[] word4;
    public string[] word5;
    public string[] word6;
}

[System.Serializable]
public class wordsModelList
{
    public List<wordsModel> words;
}

public class DosyaIslemleri : MonoBehaviour
{
    //Bölüm sonu canvaslarýný tutan obje.
    public GameObject nextLevelCanvas;

    //kelime listesi.
    public List<Kelime> wordList;

    //Json dosyasýndan alýnacak yapý için root nesnesi ve alýnan kelimelerin rahat gezmek için tutulacaðý dizi.
    public wordsModelList root;
    public string[,] rootWords = new string[6,2];

    void Awake()
    {
        if (!PlayerPrefs.HasKey("level"))
        {
            PlayerPrefs.SetInt("level", 0);
            PlayerPrefs.SetInt("hintCount", 5);
        }

        //Resources klasöründeki words.json dosyasýnýn okunmasý ve verilerin alýnmasý.
        string filePath = "words";
        TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        root = JsonUtility.FromJson<wordsModelList>(targetFile.text);

        getWordFromFile();

    }

    //Root yapýsýndan level'a göre alýnan kelimeleri rootWords dizisine atayan metod.
    public void arrayCreat(int level)
    {
        rootWords[0, 0] = root.words[level].word1[0];
        rootWords[0, 1] = root.words[level].word1[1];
        rootWords[1, 0] = root.words[level].word2[0];
        rootWords[1, 1] = root.words[level].word2[1];
        rootWords[2, 0] = root.words[level].word3[0];
        rootWords[2, 1] = root.words[level].word3[1];
        rootWords[3, 0] = root.words[level].word4[0];
        rootWords[3, 1] = root.words[level].word4[1];
        rootWords[4, 0] = root.words[level].word5[0];
        rootWords[4, 1] = root.words[level].word5[1];
        rootWords[5, 0] = root.words[level].word6[0];
        rootWords[5, 1] = root.words[level].word6[1];
    }

    //Dosyadan okunan kelimelerin, kelime listesindeki her bir kelimeye tr-ing karþýlýklarýnýn atanmasý.
    public void getWordFromFile()
    {
        
        arrayCreat(PlayerPrefs.GetInt("level"));

        for(int i=0; i<6; i++)
        {
            wordList[i].setTr(rootWords[i,0]);
            wordList[i].setIng(rootWords[i,1]);
        }
    }

    private void Update()
    {
        isFinishControl();
    }

    //Bölümün bitip bitediðini kontrol eden, bitti ise gerekli Coroutine'i baþlatan metod.
    public void isFinishControl()
    {
        foreach(var word in wordList)
        {
            if(word.isFind == false)
            {
                return;
            }
        }
        
        if (PlayerPrefs.GetInt("level") == root.words.Count-1)
        {
            StartCoroutine(nextLevelCanvasActive(1));
        }
        else
        {
            StartCoroutine(nextLevelCanvasActive(0));
        }
    }
    IEnumerator nextLevelCanvasActive(int x)
    {
        yield return new WaitForSeconds(3);
        nextLevelCanvas.transform.GetChild(x).gameObject.SetActive(true);
    }

    //Next Level Canvas'daki butona týklandýðýnda çalýþan, gerekli bütün sýfýrlamalarý çaðýran metod.
    public void nextLevel()
    {
        //Canvasý kapatýp, level'ý arttýrýyor ardýndan yeni level'a göre kelimeleri getiren metodu çaðýrýyoruz.
        nextLevelCanvas.transform.GetChild(0).gameObject.SetActive(false);
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        getWordFromFile();

        //Bütün kelimeler için unfound metodu ile sýfýrlamayý yapýyoruz.
        for (int i = 0; i < wordList.Count; i++)
        {
            wordList[i].unfound();
        }

        //Tablonun tekrar yerleþmesini saðlýyor ve coroutine'leri durduruyoruz.
        GetComponent<Tablo>().callInOrder();
        StopAllCoroutines();
    }

    //Replay ise son level'a geldiðimizde baþa dönmeyi saðlayan metod.
    public void replay()
    {
        nextLevelCanvas.transform.GetChild(1).gameObject.SetActive(false);
        PlayerPrefs.SetInt("level", -1);
        nextLevel();
    }

}
