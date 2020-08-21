#IMPORT System.Linq.dll
#IMPORT System.Core.dll
using System;
using System.Collections.Generic;
using System.Linq;

public class Macro {	
	public string LastScript;
	public string Finish(string Line, string Script){	
		try{
			if (Script != LastScript){
				LastScript = Script;
				Line += "\r\n[initscene translate=true]";
				AutoSprite.Free();
			}
			
			Line = Line.Replace("::SCRIPT::", System.IO.Path.GetFileName(Script)).Replace("::RND::", GetRandom().ToString());
			
			while (Line.Contains("\"FILE:")){
				int ID = Line.IndexOf("\"FILE:") + "\"FILE:".Length;
				int ED = Line.IndexOf("\"", ID);
				string Path = Line.Substring(ID, ED - ID);
				string FULL = string.Format("\"FILE:{0}\"", Path);
				string New = System.IO.Path.GetFileName(Path);
				Line = Line.Replace(FULL, New);
				//Console.WriteLine("REF: " + New);
			}
			
			while (Line.Contains("\"SPRITE:")){
				int ID = Line.IndexOf("\"SPRITE:") + "\"SPRITE:".Length;
				int ED = Line.IndexOf("\"", ID);
				string Value = Line.Substring(ID, ED - ID);
				string FULL = string.Format("\"SPRITE:{0}\"", Value);
				Line = Line.Replace(FULL, Value);
				string Name = GetActorName(Value);
				var Actors = AutoSprite.Add(Name);
				Line += "\r\n" + UpdateActors(Actors, Value);
				Line = Line.Replace("::AUTONAME::", Name);
			}
			
			while (Line.Contains("\"LABEL:")){
				int ID = Line.IndexOf("\"LABEL:") + "\"LABEL:".Length;
				int ED = Line.IndexOf("\"", ID);
				string Value = Line.Substring(ID, ED - ID);
				string FULL = string.Format("\"LABEL:{0}\"", Value);
				Value = LabelParse(Value, false);
				Line = Line.Replace(FULL, Value);
				
			}
			
			while (Line.Contains("\"LABELN:")){
				int ID = Line.IndexOf("\"LABELN:") + "\"LABELN:".Length;
				int ED = Line.IndexOf("\"", ID);
				string Value = Line.Substring(ID, ED - ID);
				string FULL = string.Format("\"LABELN:{0}\"", Value);
				Value = LabelParse(Value, true);
				Line = Line.Replace(FULL, Value);
				
			}
			
			if (Line.Contains("::FREESPRITE::")){
				Line = Line.Replace("::FREESPRITE::", FreeActors(AutoSprite.Free()));
			}
			
			return Line;
		
		} catch {
			Console.Write("Modifier ERROR: " + Line);
			return Line;
		}
	}
	
	public string LabelParse(string Label, bool Name){
		bool WithFlag = Label.First() == '*';
		Label = Label.TrimStart('*', ' ');
		if (Name && (Label.ToLower().EndsWith(".ks") || Label.ToLower().EndsWith(".txt")))
			Label = Label.Substring(0, Label.LastIndexOf("."));
		char First = Label.First();
		switch (First.ToString().ToUpper().First()){
			case 'B':
			case 'A':
				if (Label.Split('_').First().Length > 2)
					goto default;
				return (WithFlag ? "*" : "") + First + '0' + Label.Substring(1);
			case 'N':
				if (!Label.ToUpper().StartsWith("NEKO_") || !Name)
					goto default;
				return (WithFlag ? "*" : "") + Label.Substring(0, 7);
			default:
				return (WithFlag ? "*" : "") + Label;
		}
	}
	
	List<int> Useds = new List<int>() { 0 };
	public int GetRandom(){
		int val = Useds[Useds.Count-1];
		while (Useds.Contains((val = (new Random(val).Next(0, int.MaxValue)))))
			continue;
		Useds.Add(val);
		return val;
	}
	
	//s005_1ba1ba_032a_s
	public string GetActorName(string File){
		string Actor = File.Split('_')[0];
		while (!char.IsNumber(Actor[0])){
			Actor = Actor.Substring(1);
		}
		return "ch_" + Actor;
	}
	
	public string UpdateActors(Dictionary<string, int> Actors, string Storage){
		string Result = "";
		foreach (var Pair in Actors){
			Result += "[Object name="+ Pair.Key +" xpos="+Pair.Value+"]\r\n";
		}
		return Result;
	}
	
	public string FreeActors(string[] Actors){
		string Result = "";
		foreach (var Actor in Actors){
			Result += "[Object name="+ Actor +" hide]\r\n";
		}
		return Result;
	}
	
}

public static class AutoSprite {
	const int MaxWidth = 480*2;
	const int CharWidth = 100;
	static Dictionary<string, int> Actors = new Dictionary<string, int>();
	
	public static Dictionary<string, int> Add(string Name){
		Actors[Name] = 0;
		UpdatePos();
		return Actors;
	}
	
	static void UpdatePos() {
		int Pos = 0;
		int[] PosList = new int[Actors.Count];
		for (int i = 0; i < PosList.Length; i++){
			PosList[i] = Pos;
			Pos += CharWidth;
		}
		int TotalHalf = Pos/2;
		int PosHalf = (MaxWidth/(PosList.Length + 1));
		for (int i = 0; i < PosList.Length; i++){
			PosList[i] += PosHalf;
		}
		for (int i = 0; i < PosList.Length; i++){
			Actors[Actors.ElementAt(i).Key] = PosList[i];
		}
	}
	
	public static Dictionary<string, int> Del(string Name){
		if (Actors.ContainsKey(Name))
			Actors.Remove(Name);
		return Actors;
	}
	
	public static string[] Free(){
		var Rst = Actors.Keys.ToArray();
		Actors = new Dictionary<string, int>();
		return Rst;
	}
}