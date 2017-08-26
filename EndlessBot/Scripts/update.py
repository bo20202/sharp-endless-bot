import subprocess
import argparse
import os
from misc import *


# Getting the args
parser = argparse.ArgumentParser()
parser.add_argument("repo_dir", type=str, help="directory containing repository")
parser.add_argument("live_dir", type=str, help="directory containing live server")
parser.add_argument("-l", "--log", type=str, help="file for outputting log to")

args = parser.parse_args()


# Pull new commits
os.chdir(args.repo_dir)

git_process = subprocess.Popen(["git", "pull"], stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
git_stdout, git_stderr = git_process.communicate()
if git_process.returncode != 0:
	fail("Git failed to pull new commits. Exiting.\nLogs from git:\n{}".format(git_stdout.decode()), args.log, "a")


# Compiling the build
dme = None
files = os.listdir()

for somefile in files:
	if somefile[-4:] == ".dme":
		if not dme: dme = somefile
		else: fail("Expected 1 DME file, found more. Exiting.", args.log, "a")

if not dme:
	fail("DME file not found. Exiting.", args.log, "a")

dm_process = subprocess.Popen(["DreamMaker", dme], stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
dm_stdout, dm_stderr = dm_process.communicate()
if dm_process.returncode != 0:
	fail("DM failed to compile the build. Exiting.\nLogs from DM:\n{}".format(dm_stdout.decode()), args.log, "a")


# Copying ready files
cp_process = subprocess.Popen(["cp", "-rf", repo_dir, live_dir], stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
cp_stdout, cp_stderr = cp_process.communicate()
if cp_process.returncode != 0:
	fail("cp failed to copy build files. Exiting.\nLogs from cp:\n{}".format(cp_stdout.decode()), args.log, "a")


success("Build updated. Task completed.")
