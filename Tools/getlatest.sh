#!/bin/bash

latesturl="https://github.com/mickew/aheat/releases/latest"
rooturl="https://github.com/mickew/aheat/releases/download"
filename="release.zip"
out="latest"

#Check if script is being run as root
if [ "$(id -u)" != "0" ]; then
   echo "This script must be run as root" 1>&2
   exit 1
fi

if [ ! $? = 0 ]; then
   exit 1
else

  rm -f $filename
  rm -f -r $out

  location=$(curl -s -I $latesturl | grep -i ^Location: | cut -d: -f2- | sed 's/^ *\(.*\).*/\1/')

  version="${location##*/}"
  size=${#version}

  ver=${version:0:size-1} 

  url="$rooturl"/"$ver"/"$filename"

  # printf "Redirect-url: %s\n\n" "$location"
  # printf "Version: %s\n\n" "$version"
  # printf "Url: %s\n\n" "$url"

  curl -L -O $url

  unzip -a -q -d $out/ $filename

  mkdir -p /var/www/powercontrol
  sudo chmod 666 /var/www/powercontrol

  cp -a ./$out/* /var/www/powercontrol 

  rm -f $filename
  rm -f -r $out

fi
