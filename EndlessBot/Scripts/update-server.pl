#!/usr/bin/perl
use strict;
use warnings;
use Getopt::Long;
use File::Copy::Recursive qw(dircopy);
use File::Path qw(remove_tree);

my $cdir;
my $pdir;
my $odir;
my $lsym;
if(@ARGV < 5){
   die("Usage: perl script.pl\n --lsym live-symlink-path\n --cdir current-dir-path\n --pdir previous-dir-path\n --odir oldest-dir-path");
}

GetOptions("cdir=s" => \$cdir,
	   "pdir=s" => \$pdir,
	   "odir=s" => \$odir,
	   "lsym=s" => \$lsym)

or die("Error in command line\n");

print("Copying current directory to previous.\n"); 
dircopy($cdir, $pdir);

print("Updating current version.\n."); 
system("git", "-C", "$cdir", "pull");

print("Compiling current version.\n"); 
system("DreamMaker", "$cdir/cev_eris.dme");

print("Copying data and config folders\n"); 
dircopy("$pdir/config", "$cdir");
dircopy("$pdir/data", "$cdir");

print("Creating symlink to current version");
system("ln", "-sfn", "$cdir", "$lsym");
	
print("Removing oldest directory.\n"); 
remove_tree($odir);
