import os
import zipfile
# kill flow launcher process
import psutil
for proc in psutil.process_iter():
    if proc.name() == 'Flow.Launcher.exe':
        proc.kill()
        print('Flow Launcher process killed')
        break

from dotenv import load_dotenv
load_dotenv()
print(os.environ['FLOW_LAUNCHER_PLUGIN_PATH'])
input("make sure you have the correct path")

# get the zip from the project
zip_file = zipfile.ZipFile('RegFlow/bin/RegFlow.zip', 'r')

# delete existing plugin folder
import shutil
shutil.rmtree(os.environ['FLOW_LAUNCHER_PLUGIN_PATH'] + '/RegFlow', ignore_errors=True)

# extract the files from the zip
zip_file.extractall(os.environ['FLOW_LAUNCHER_PLUGIN_PATH'])
os.rename(os.environ['FLOW_LAUNCHER_PLUGIN_PATH'] + '/publish', os.environ['FLOW_LAUNCHER_PLUGIN_PATH'] + '/RegFlow')