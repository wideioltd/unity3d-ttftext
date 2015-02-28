#!/usr/bin/env python
import os,sys
import shutil
import subprocess

jar_name = 'TTFTextAndroidSystFont.jar'

cwd=os.getcwd()
os.chdir("android")
"""
set JAVA_HOME="C:\Progra~1\Java\Jdk1.70_05"
"""
#mkdir libs
#FileUtils.cp(android_classes, 'libs')

try:
  subprocess.check_call([r"c:\Program Files (x86)\Android\android-sdk\tools\android.bat" ,"update", "project", "--target", "android-10", "-p", "."])
  subprocess.check_call([r"c:\Sources\apache-ant-1.8.4\bin\ant.bat", "release"])
  os.chdir(cwd)
  #shutil.copy2("android/bin/classes.jar" ,"Assets/TTFText/TTFText/Scripts/Plugins/Android/TTFTextAndroidSysFont.jar")
  shutil.copy2("android/bin/classes.jar" ,"Assets/Plugins/Android/TTFTextAndroidSysFont.jar")
except Exception,e:
  print e

# "c:\Program Files (x86)\Android\android-sdk\tools\emulator.exe"
sys.stdin.readline()
