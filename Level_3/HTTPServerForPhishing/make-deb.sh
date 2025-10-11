#!/bin/bash 

#make deb package 
_PACKAGE = main 
_VERSION = 1.0.0
_ARCH = "ALL"
PKG_NAME = "${_PACKAGE}${_VERSION}${_ARCH}.deb"

if [[ ! -e "scripts/launch.sh"]]; then 
  echo "launch.sh shoild be in the /'scripts/ directory. Exiting..."
  exit 1
fi 

if [[ ${1,,} == "termux" || $(uname -o) == *'Android'*]]; then 
  _depend = "ncurses-utils, proot, resolv-conf, "
  _bin_dir = "data/data/com.termux/files/"
  _opt_dir = "data/data/com.termux/files/usr/"
fi 

_depend += "curl, php, unzip"
_bin_dir += "usr/bin"
_opt_dir += "opt/${_PACKAGE}"

if [[ -d "build_env"]]; then rm -rf build_env; fi 
mkdir -p build_env
mkdir -p ./build_env/${_bin_dir} ./build_env/$_opt_dir ./build_env/DEBIAN

cat <<- CONTROL_EOF > ./build_env/DEBIAN/control 
Package: ${_PACKAGE}
Architecture: ${_ARCH}
Maintainer: @RAMMLS 
Depends: ${_depend}
CONTROL_EOF

cat <<- PRERM_EOF > ./build_env/DEBIAN/prerm 
#!/bin/bash 
rm -rf $_opt_dir
exit 0 
PRERM_EOF

chmod 755 ./build_env/DEBIAN 
chmod 755 ./build_env/DEBIAN/{control, prerm}
cp -fr scripts/launch.sh ./build_env/$_bin_dir/$_PACKAGE
chmod 755 ./build_env/$_bin_dir/$_PACKAGE
cp -fr .github/ .sites/ README.md main.sh ./build_env/$_opt_dir
dpkg-deg --build ./build_env ${PKG_NAME}
rm -rf ./build_env
