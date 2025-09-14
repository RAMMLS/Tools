The Reflected XSS Vulnerability Scanner is a tool designed to help security professionals and developers identify reflected XSS vulnerabilities in web applications. By scanning a list of URLs provided in a .txt file, this tool can efficiently detect potential security flaws that might be exploited by attackers.

It's important to note that no vulnerability scanner is 100% accurate; ultimately, you still need to reproduce the vulnerability manually to create the proof of concept (PoC). This script simply helps detect which parameter values in the URLs lack input validation and may be vulnerable to reflected XSS.

Features

    Efficiency: The tool scans multiple URLs concurrently, significantly reducing the time required to identify vulnerabilities.

    Customization: Users can easily modify the payload to test for various types of XSS attacks, making the tool adaptable to different testing scenarios.

    Reliability: With robust error handling and support for HTTP/2, the scanner ensures accurate results even in complex environments.

    Stealth: By rotating through a list of User-Agent strings, the tool mimics real browser requests, reducing the likelihood of detection during scans.

This tool is an essential addition to any security professional's toolkit, providing a quick and effective way to improve the security posture of web applications.

go build main.go
