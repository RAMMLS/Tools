ℹ️ Running the code

To run this code, follow these steps:

    Install the project's dependencies using the requirements.txt file:

pip3 install -r requirements.txt

    Run the main.py script:

python3 main.py

    To run a command, you can add it as a parameter in the URL, along with a basic username and password authentication, here's an example curl:

curl -u admin:password -X GET -i http://localhost:5000/?command=ls -la

If the command runs successfully, you will see the command output in the browser. If an error occurs, you will see an error message.
