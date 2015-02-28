#!/usr/bin/env python
import subprocess
import os
import stat
import shutil
import sys

try:
  os.chdir("Assets\\TTFText\\TTFText\\Scripts")

  UP=None
  UnityPaths=["C:\\Program Files (x86)\\Unity\\Editor","C:\\Program Files\\Unity\\Editor","C:\\Program Files (x86)\\Unity353\\Editor","C:\\Program Files\\Unity353\\Editor"]
  for cup in UnityPaths:
    try:
      os.stat(cup)
      UP=cup
      break
    except:
      pass

  try:
    os.stat("C:\\builds\\TTFText Deluxe\\Assets\\TTFText\\TTFText\\Plugins")
  except:
    os.makedirs("C:\\builds\\TTFText Deluxe\\Assets\\TTFText\\TTFText\\Plugins")
  try:
    os.stat("C:\\builds\\TTFText Deluxe\\Assets\\TTFText\\TTFText\\Editor")
  except:
    os.makedirs("C:\\builds\\TTFText Deluxe\\Assets\\TTFText\\TTFText\\Editor")	
  try:
    os.stat("C:\\builds\\TTFText Lite\\Assets\\TTFText\\TTFText\\Plugins")
  except:
    os.makedirs("C:\\builds\\TTFText Lite\\Assets\\TTFText\\TTFText\\Plugins")
  try:
    os.stat("C:\\builds\\TTFText Lite\\Assets\\TTFText\\TTFText\\Editor")
  except:
    os.makedirs("C:\\builds\\TTFText Lite\\Assets\\TTFText\\TTFText\\Editor")
  #editor_only=["ttftextstyle.cs"]
  editor_only=[]
  base_files=map(lambda x:"Plugins/"+x,filter(lambda x:x.lower() not in editor_only,filter(lambda x:x.endswith(".cs"),os.listdir("plugins"))))
  internal_files=map(lambda x:"Plugins/internal/"+x,filter(lambda x:x.endswith(".cs"),os.listdir("plugins/internal")))
  internal_files+=map(lambda x:"Plugins/internal/fontengines/"+x,filter(lambda x:x.endswith(".cs"),os.listdir("plugins/internal/fontengines")))
  base_files+=map(lambda x:"extra/"+x,filter(lambda x:x.endswith(".cs"),os.listdir("extra")))  
  editor_files=map(lambda x:"editor/"+x,filter(lambda x:x.endswith(".cs"),os.listdir("editor")))
  editor_files+=map(lambda x:"Plugins/"+x,filter(lambda x:x.lower() in editor_only,filter(lambda x:x.endswith(".cs"),os.listdir("plugins"))))
  options=["-platform:anycpu","-debug-","-optimize+"]
  libraries_free=["ttftext_3p_freetype_cs.dll","ttftext_3p_triangulation_glu.dll"]
  libraries=["ttftext_3p_freetype_cs.dll","ttftext_3p_triangulation_glu.dll","ttftext_3p_triangulation_poly2tri.dll","ttftext_3p_clipper.dll","ttftext_3p_expression_parser.dll"]
  
  
  subprocess.check_call([UP+"\\Data\\Mono\\bin\\gmcs.bat","-target:library","-lib:..\\Plugins,"+UP+"\\Data\\Managed"]+options+["-r:UnityEngine.dll","-r:UnityEditor.dll"]+["-r:"+s for s in libraries_free]+["-define:UNITY_EDITOR","-define:TTFTEXT_LITE", "-out:C:\\builds\\TTFText Lite\\Assets\\TTFText\\TTFText\\Plugins\\ttftext_lite.dll" ]+base_files)
  subprocess.check_call([UP+"\\Data\\Mono\\bin\\gmcs.bat","-target:library","-lib:..\\Plugins,"+UP+"\\Data\\Managed,C:\\builds\\TTFText Lite\\Assets\\TTFText\\TTFText\\Plugins\\"]+options+["-r:UnityEngine.dll","-r:UnityEditor.dll"]+["-r:"+s for s in libraries_free]+["-r:ttftext_lite.dll","-define:UNITY_EDITOR","-define:TTFTEXT_LITE", "-out:C:\\builds\\TTFText Lite\\Assets\\TTFText\\TTFText\\Editor\\ttftext_lite_editor.dll" ] +editor_files+internal_files)  
  for l in libraries_free:
	shutil.copy2("..\\Plugins\\"+l,"C:\\builds\\TTFText Lite\\Assets\\TTFText\\TTFText\\Plugins\\"+l)
 # subprocess.check_call([UP+"\\Data\\Mono\\bin\\gmcs.bat","-target:library","-lib:..\\Plugins,"+UP+"\\Data\\Managed"]+options+["-r:UnityEngine.dll"]+["-r:"+s for s in libraries]+["-define:UNITY_EDITOR","-define:TTFTEXT_DELUXE", "-out:C:\\builds\\TTFText Deluxe\\Assets\\TTFText\\TTFText\\Plugins\\ttftext_deluxe.dll" ] +base_files+internal_files)
 # subprocess.check_call([UP+"\\Data\\Mono\\bin\\gmcs.bat","-target:library","-lib:..\\Plugins,"+UP+"\\Data\\Managed,C:\\builds\\TTFText Deluxe\\Assets\\TTFText\\TTFText\\Plugins\\"]+options+["-r:UnityEngine.dll","-r:UnityEditor.dll"]+["-r:"+s for s in libraries]+["-r:ttftext_deluxe.dll","-define:UNITY_EDITOR","-define:TTFTEXT_DELUXE", "-out:C:\\builds\\TTFText Deluxe\\Assets\\TTFText\\TTFText\\Editor\\ttftext_deluxe_editor.dll" ] +editor_files)

except Exception,e:
  print e
  pass  
  
sys.stdin.readline()
