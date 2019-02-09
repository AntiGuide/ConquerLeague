using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(TextMeshProUGUI))]
public class KillfeedManager : MonoBehaviour {
    public enum DeathCategory : int {
        MG = 0,
        TOWER = 1,
        ULTIMATE = 2,
    }

    public struct KillFeedEvent {
        TeamHandler.TeamState murderer;
        string murdererName;
        DeathCategory category;
        TeamHandler.TeamState victim;
        string victimName;

        public KillFeedEvent(TeamHandler.TeamState murderer, DeathCategory category, TeamHandler.TeamState victim) {
            this.murdererName = murderer == TeamHandler.TeamState.FRIENDLY ? "Player1" : "Player2";
            this.murderer = murderer;
            this.category = category;
            this.victim = victim;
            this.victimName = victim == TeamHandler.TeamState.FRIENDLY ? "Player1" : "Player2";
        }

        public string ToString(string trans = "FF") {
            var sb = new StringBuilder();
            sb.Append("<color=#");
            sb.Append(murderer == TeamHandler.TeamState.FRIENDLY ? "3C5EFF" : "FF0000");
            sb.Append(trans);
            sb.Append(">");
            sb.Append(murdererName);
            sb.Append("</color> <sprite index=");
            sb.Append((int)category);
            sb.Append(" color=#FFFFFF");
            sb.Append(trans);
            sb.Append("> <color=#");
            sb.Append(victim == TeamHandler.TeamState.FRIENDLY ? "3C5EFF" : "FF0000");
            sb.Append(trans);
            sb.Append(">");
            sb.Append(victimName);
            sb.Append("</color>");
            return sb.ToString();
        }

        public override string ToString() {
            return ToString("FF");
        }
    }

    [SerializeField] private int maxLines = 5;
    public static KillfeedManager FS;
    private TextMeshProUGUI tmp;
    private StringBuilder sb = new StringBuilder();
    private List<KillFeedEvent> feed = new List<KillFeedEvent>();

    public void AddDeathEvent(KillFeedEvent killFeedEvent) {
        feed.Insert(0, killFeedEvent);
        StartCoroutine(RemoveLast());
    }

    public void AddDeathEvent(TeamHandler.TeamState murderer, DeathCategory category, TeamHandler.TeamState victim) {
        AddDeathEvent(new KillFeedEvent(murderer, category, victim));
    }

    private IEnumerator RemoveLast() {
        yield return new WaitForSeconds(3f);
        feed.RemoveAt(feed.Count - 1);
    }

    // Use this for initialization
    void Start () {
        if (FS != null) {
            Application.Quit();
        }

        FS = this;
        tmp = GetComponent<TextMeshProUGUI>();
    }
	
	// Update is called once per frame
	void Update () {
        sb.Clear();
        var lines = feed.Count < maxLines ? feed.Count : maxLines;
        for (int i = 0; i < lines; i++) {
            var transparency = i < maxLines - 1 ? 0x88 : 0x44;
            transparency = i < maxLines - 2 ? 0xFF : transparency;
            var tranStr = transparency.ToString("X2");
            sb.AppendLine(feed[i].ToString(tranStr));
        }

        tmp.text = sb.ToString();
    }
}
