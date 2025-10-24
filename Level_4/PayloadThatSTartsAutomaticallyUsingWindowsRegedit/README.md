Компиляция на Linux:

x86_64-w64-mingw32-g++ -static -o program.exe program.cpp

Запуск питон-файла: 

python3 -m venv venv 
cd venv 
source /bin/activate 

pip install -r requirements.txt


################################
# Собираем образ
docker build -t cross-compiler .

# Запуск с автоматической сборкой тестовых программ
docker run -it --rm cross-compiler build-all.sh

# Компиляция c++ файла
docker run --rm -v $(pwd):/workspace cross-compiler \
  x86_64-w64-mingw32-g++ -static -o /workspace/program.exe /workspace/program.cpp


# После сборки образа выполните:
docker run --rm cross-compiler build-all.sh


'''
=== Testing C++ Compilation ===
x86_64-w64-mingw32-g++ (GCC) ...
=== Testing Go Compilation ===
go version go1.21.0 ...
=== Building Test Programs ===
Building C++...
Building Go...
=== Verification ===
/workspace/hello-cpp.exe: PE32+ executable (console) x86-64...
/workspace/hello-go.exe: PE32+ executable (console) x86-64...
=== Build Complete ===
-rwxr-xr-x 1 root root ... /workspace/hello-cpp.exe
-rwxr-xr-x 1 root root ... /workspace/hello-go.exe
'''
