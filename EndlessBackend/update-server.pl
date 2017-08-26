#!/usr/bin/perl
use strict;
use warnings;
use Getopt::Long;
use File::Copy::Recursive qw(dircopy);
use File::Path qw(remove_tree);
require IPC::System::Simple;
use autodie qw(system);

my $cdir;
my $pdir;
my $lsym;
if(@ARGV < 3){
   die("Usage: perl script.pl\n --lsym live-symlink-path\n --cdir current-dir-path\n --pdir previous-dir-path\n");
}

GetOptions("cdir=s" => \$cdir,
           "pdir=s" => \$pdir,
           "lsym=s" => \$lsym)

or die("Error in command line\n");

print("Copying previous to new.\n");
dircopy($pdir, $cdir) or die("Failed to copy previous to new");

print("Updating current version.\n");
system("git", "-C", "$cdir", "pull", "-q");

print("Compiling current version.\n");
system("DreamMaker", "$cdir/cev_eris.dme");

print("Copying data and config folders\n");
dircopy("$pdir/config", "$cdir") or die "Failed to copy config";
dircopy("$pdir/data", "$cdir") or die "Failed to copy data";

print("Creating symlink to current version");
system("ln", "-sfn", "$cdir", "$lsym");

exit 0;



