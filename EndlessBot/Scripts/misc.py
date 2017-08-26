import sys, time

# --- Functions defines

# Successful exiting
def success(message=None, logfile=None, mode=None, exitcode=0):
	outstring = "[SUCCESS] :: Task completed successfully, exiting." if message == None else "[SUCCESS] :: "+message
	print(outstring)
	if logfile: open(logfile, mode).write(time.asctime()+" "+outstring+'\n')
	sys.exit(exitcode)

# Failure exiting
def fail(message=None, logfile=None, mode=None, exitcode=1):
	outstring = "[FAIL] :: Task failed, exiting." if message == None else "[FAIL] :: "+message
	print(outstring)
	if logfile: open(logfile, mode).write(time.asctime()+" "+outstring+'\n')
	sys.exit(exitcode)

# Logging
def log(message, logfile=None, mode=None):
	outstring = "[INFO] :: "+message
	print(outstring)
	if logfile: open(logfile, mode).write(time.asctime()+" "+outstring+'\n')
