using UnityEngine;
using System.Collections;

[AddComponentMenu("Text/TTFText DemoScenes Helpers/LongText Demo Main")]
public class TextGenDemoMain : MonoBehaviour {
	
	public GameObject prefab;
	
	public static string [] text={
		"The representatives of the French people, organized as a National Assembly, believing that the ignorance, neglect, or contempt of the rights of man are the sole cause of public calamities and of the corruption of governments, have determined to set forth in a solemn declaration the natural, unalienable, and sacred rights of man, in order that this declaration, being constantly before all the members of the Social body, shall remind them continually of their rights and duties; in order that the acts of the legislative power, as well as those of the executive power, may be compared at any moment with the objects and purposes of all political institutions and may thus be more respected, and, lastly, in order that the grievances of the citizens, based hereafter upon simple and incontestable principles, shall tend to the maintenance of the constitution and redound to the happiness of all. Therefore the National Assembly recognizes and proclaims, in the presence and under the auspices of the Supreme Being, the following rights of man and of the citizen:",
	    "Articles:",
        "1.  Men are born and remain free and equal in rights. Social distinctions may be founded only upon the general good.",
        "2.  The aim of all political association is the preservation of the natural and imprescriptible rights of man. These rights are liberty, property, security, and resistance to oppression.",
		"3.  The principle of all sovereignty resides essentially in the nation. No body nor individual may exercise any authority which does not proceed directly from the nation.",
		"4.  Liberty consists in the freedom to do everything which injures no one else; hence the exercise of the natural rights of each man has no limits except those which assure to the other members of the society the enjoyment of the same rights. These limits can only be determined by law.",
		"5.  Law can only prohibit such actions as are hurtful to society. Nothing may be prevented which is not forbidden by law, and no one may be forced to do anything not provided for by law.",
		"6.  Law is the expression of the general will. Every citizen has a right to participate personally, or through his representative, in its foundation. It must be the same for all, whether it protects or punishes. All citizens, being equal in the eyes of the law, are equally eligible to all dignities and to all public positions and occupations, according to their abilities, and without distinction except that of their virtues and talents.",
		"7.  No person shall be accused, arrested, or imprisoned except in the cases and according to the forms prescribed by law. Any one soliciting, transmitting, executing, or causing to be executed, any arbitrary order, shall be punished. But any citizen summoned or arrested in virtue of the law shall submit without delay, as resistance constitutes an offense.",
		"8.  The law shall provide for such punishments only as are strictly and obviously necessary, and no one shall suffer punishment except it be legally inflicted in virtue of a law passed and promulgated before the commission of the offense.",
		"9.  As all persons are held innocent until they shall have been declared guilty, if arrest shall be deemed indispensable, all harshness not essential to the securing of the prisoner's person shall be severely repressed by law.",
		"10.  No one shall be disquieted on account of his opinions, including his religious views, provided their manifestation does not disturb the public order established by law.",
		"11.  The free communication of ideas and opinions is one of the most precious of the rights of man. Every citizen may, accordingly, speak, write, and print with freedom, but shall be responsible for such abuses of this freedom as shall be defined by law.",
		"12.  The security of the rights of man and of the citizen requires public military forces. These forces are, therefore, established for the good of all and not for the personal advantage of those to whom they shall be intrusted.",
		"13.  A common contribution is essential for the maintenance of the public forces and for the cost of administration. This should be equitably distributed among all the citizens in proportion to their means.",
		"14.  All the citizens have a right to decide, either personally or by their representatives, as to the necessity of the public contribution; to grant this freely; to know to what uses it is put; and to fix the proportion, the mode of assessment and of collection and the duration of the taxes.",
		"15.  Society has the right to require of every public agent an account of his administration.",
		"16.  A society in which the observance of the law is not assured, nor the separation of powers defined, has no constitution at all.",
		"17.  Since property is an inviolable and sacred right, no one shall be deprived thereof except where public necessity, legally determined, shall clearly demand it, and then only on condition that the owner shall have been previously and equitably indemnified."
		
	};
	
	
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (clicked) {
				GameObject tgo=new GameObject();
		tgo.AddComponent<TTFText>();
		tgo.transform.localPosition=new Vector3(0,3.40f,0);
		tgo.transform.localRotation=Quaternion.Euler(40,0,0);
		TTFText tm=tgo.GetComponent<TTFText>();
		tm.FontId="Droid Sans Mono (Regular)";
		tm.Size=0.25f;
		tm.LineWidth=15;
		tm.LayoutMode=TTFText.LayoutModeEnum.Wrap;
		tm.ParagraphAlignment=TTFText.ParagraphAlignmentEnum.Justified;
		tm.TokenMode=TTFText.TokenModeEnum.Word;
		tm.GlyphPrefab=prefab;
		tm.FirstLineOffset=1.5f;
	//	tm.DynamicTextRuntimeFontProviderMethod=TTFText.DynamicTextRuntimeFontProviderMethodEnum.EmbeddedAndNetworkFonts;
		tm.Text=string.Join("\n",text);//.Substring(0,1000);
				started=true;
			clicked=false;
		}
	
	}
	
	bool clicked=false;
	bool started=false;
	
	void OnGUI() {
		
		if (!started) {
			if (GUI.Button(new Rect(0,0,Screen.width,Screen.height),"Click here and wait for a few seconds...")) {
				clicked=true;
			}
		}
	}
}
