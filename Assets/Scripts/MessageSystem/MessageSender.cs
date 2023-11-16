using System;
using UnityEngine;

[Serializable]
public class MessageSender
{
    public string Name { get; }
    public string IconSpriteID { get; }
    public Sprite IconSprite => MessageSystemManager.Instance.GetIconSprite(IconSpriteID);

    public MessageSender(string name, string iconSpriteID)
    {
        Name = name;
        IconSpriteID = iconSpriteID;
    }
}
