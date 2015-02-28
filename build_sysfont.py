#!/usr/bin/env python
import os,sys
import shutil
import subprocess

jar_name = 'NormalSystFont.jar'

cwd=os.getcwd()
os.chdir("sysfont/android")
"""
set JAVA_HOME="C:\Progra~1\Java\Jdk1.70_05"
"""

try:
  subprocess.check_call([r"c:\Program Files (x86)\Android\android-sdk\tools\android.bat" ,"update", "project", "--target", "android-10", "-p", "."])
  subprocess.check_call([r"c:\Sources\apache-ant-1.8.4\bin\ant.bat", "release"])
  os.chdir(cwd)
  shutil.copy2("sysfont/android/bin/classes.jar" ,"Assets/Plugins/Android/NormalSysFont.jar")
except Exception,e:
  print e


sys.stdin.readline()
