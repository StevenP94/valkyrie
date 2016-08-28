﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroSelection {

	public HeroSelection(Game.Hero h)
    {
        Game game = Game.Get();

        DialogBox db = new DialogBox(new Vector2(8, 1), new Vector2(UIScaler.GetWidthUnits() - 16, 3), "Select Heros");
        db.textObj.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetLargeFont();

        float x = 8;
        float y = 5;

        HeroSelectButton(new Vector2(x, y), null, h.id);
        foreach (KeyValuePair<string, HeroData> hd in game.cd.heros)
        {
            x += 6;
            if (x > UIScaler.GetRight(-13))
            {
                x = 8;
                y += 6;
            }

            bool disabled = false;
            foreach (Game.Hero hIt in game.heros)
            {
                if ((hIt.heroData == hd.Value) && (hIt.id != h.id))
                {
                    disabled = true;
                }
            }
            HeroSelectButton(new Vector2(x, y), hd.Value, h.id, disabled);
        }
	}

    public void HeroSelectButton(Vector2 position, HeroData hd, int id, bool disabled = false)
    {
        Sprite heroSprite;
        Texture2D newTex = Resources.Load("sprites/tokens/objective-token-black") as Texture2D;
        string name = "";

        if (hd != null)
        {
            string imagePath = @"file://" + hd.image;
            WWW www = new WWW(imagePath);
            newTex = new Texture2D(256, 256, TextureFormat.DXT5, false);
            www.LoadImageIntoTexture(newTex);
            name = hd.name;
        }

        GameObject heroImg = new GameObject("heroImg" + name);
        heroImg.tag = "dialog";

        Game game = Game.Get();
        heroImg.transform.parent = game.uICanvas.transform;

        RectTransform trans = heroImg.AddComponent<RectTransform>();
        trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, position.y * UIScaler.GetPixelsPerUnit(), 5f * UIScaler.GetPixelsPerUnit());
        trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, position.x * UIScaler.GetPixelsPerUnit(), 5f * UIScaler.GetPixelsPerUnit());
        heroImg.AddComponent<CanvasRenderer>();


        UnityEngine.UI.Image image = heroImg.AddComponent<UnityEngine.UI.Image>();
        heroSprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), Vector2.zero, 1);
        image.sprite = heroSprite;
        image.rectTransform.sizeDelta = new Vector2(5f * UIScaler.GetPixelsPerUnit(), 5f * UIScaler.GetPixelsPerUnit());

        UnityEngine.UI.Button button = heroImg.AddComponent<UnityEngine.UI.Button>();
        button.interactable = !disabled;
        button.onClick.AddListener(delegate { SelectHero(id, name); });
    }

    public void SelectHero(int id, string name)
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("dialog"))
            Object.Destroy(go);

        Game game = Game.Get();
        HeroData hData = null;
        foreach (KeyValuePair<string, HeroData> hd in game.cd.heros)
        {
            if (hd.Value.name.Equals(name))
            {
                hData = hd.Value;
            }
        }
        foreach (Game.Hero h in game.heros)
        {
            if (h.id == id)
            {
                h.heroData = hData;
            }
        }

        game.heroCanvas.UpdateImages();
    }
}
