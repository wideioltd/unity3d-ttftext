#!/usr/bin/env python
import subprocess
import os
import stat
import shutil
import sys

def allcs(path):
  r=[]
  for f in os.listdir(path):
     ln=os.path.join(path,f)
     sr=os.stat(ln)
     if (stat.S_ISDIR(sr[0])):
       r+=allcs(ln)
     else:
       if (f.endswith(".cs")):
         r.append(ln)
  return r

try:
  UP=None
  UnityPaths=["C:\\Program Files (x86)\\Unity\\Editor","C:\\Program Files\\Unity\\Editor"]
  for cup in UnityPaths:
    try:
      os.stat(cup)
      UP=cup
      break
    except:
      pass

  base_files=allcs("ftsharp")
  print base_files
  options=["-platform:anycpu","-debug-","-optimize+","-unsafe"]
  subprocess.check_call([UP+"\\Data\\Mono\\bin\\gmcs.bat","-target:library","-lib:..\\Plugins,"+UP+"\\Data\\Managed"]+options+["-r:UnityEngine.dll" , "-out:Assets\\TTFText\\TTFText\\Plugins\\ttftext_3p_freetype_cs.dll" ]+base_files)
  base_files=allcs("triangulation")
  subprocess.check_call([UP+"\\Data\\Mono\\bin\\gmcs.bat","-target:library","-lib:..\\Plugins,"+UP+"\\Data\\Managed"]+options+["-r:UnityEngine.dll" , "-out:Assets\\TTFText\\TTFText\\Plugins\\ttftext_3p_triangulation_glu.dll" ]+base_files)
  base_files=allcs("parseexpr")
  subprocess.check_call([UP+"\\Data\\Mono\\bin\\gmcs.bat","-target:library","-lib:..\\Plugins,"+UP+"\\Data\\Managed"]+options+["-r:UnityEngine.dll" , "-out:Assets\\TTFText\\TTFText\\Plugins\\ttftext_3p_expression_parser.dll" ]+base_files)
  base_files=allcs("P2T")
  subprocess.check_call([UP+"\\Data\\Mono\\bin\\gmcs.bat","-target:library","-lib:..\\Plugins,"+UP+"\\Data\\Managed"]+options+["-r:UnityEngine.dll" , "-out:Assets\\TTFText\\TTFText\\Plugins\\ttftext_3p_triangulation_poly2tri.dll" ]+base_files)
  base_files=allcs("clipper")
  subprocess.check_call([UP+"\\Data\\Mono\\bin\\gmcs.bat","-target:library","-lib:..\\Plugins,"+UP+"\\Data\\Managed"]+options+["-r:UnityEngine.dll" , "-out:Assets\\TTFText\\TTFText\\Plugins\\ttftext_3p_clipper.dll" ]+base_files)
except Exception,e:
  print e
  pass  
  
sys.stdin.readline()
