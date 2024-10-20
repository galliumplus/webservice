import os, sys
import subprocess
import http.client
import requests
import time
import unittest
import psutil, signal

from utils.test_base import TestBase


class Launcher:
    ANSI_RED = "\x1b[91m"
    ANSI_YELLOW = "\x1b[93m"
    ANSI_GREEN = "\x1b[32m"
    ANSI_RESET = "\x1b[0m"

    @classmethod
    def launch(cls, targeted_version="0.0.0"):
        if "--help" in sys.argv:
            print(
                """
options specific to gallium tests:
  --manual        Doesn't start the server automatically
  --long-timeout  Waits 2 minutes for the server to start
"""
            )
            unittest.main()

        auto = "--manual" not in sys.argv
        if not auto:
            sys.argv.remove("--manual")

        if "--long-timeout" in sys.argv:
            sys.argv.remove("--long-timeout")
            timeout = 60
        else:
            timeout = 10

        if "--no-restore" in sys.argv:
            sys.argv.remove("--no-restore")
            no_restore = True
        else:
            no_restore = False

        success = False

        # free port
        if auto:
            try:
                cls.free_port(5443)
            except Exception as err:
                print(
                    f"{cls.ANSI_RED}Failed to free port 5443 because {type(err).__name__} {err}.{cls.ANSI_RESET}"
                )
                sys.exit(1)

        # start test server
        if auto:
            print("Starting server in TEST mode...")
            args = ["dotnet", "run", "--project", "WebService", "-c", "Test"]
            if no_restore:
                args.append("--no-restore")
            server = subprocess.Popen(args)
        else:
            print("Waiting for server to start...")

        # wait for server
        ping_client = http.client.HTTPConnection("localhost", 5443)

        for n in range(timeout):
            try:
                ping_client.connect()
                print("Server is up !")
                ping_client.close()

                if cls.compver(targeted_version):
                    exit(1)

                break

            except ConnectionRefusedError:
                if auto and server.poll():
                    print(f"{cls.ANSI_RED}Failed to start server.{cls.ANSI_RESET}")
                    sys.exit(1)
                else:
                    print(f"Server not up yet, retrying soon... ({n+1}/{timeout})")
                    time.sleep(2)
        else:
            print(f"{cls.ANSI_RED}Timed out.{cls.ANSI_RESET}")
            sys.exit(1)

        # run tests
        try:
            result = unittest.main(exit=False).result
            success = result.wasSuccessful()
        except Exception as e:
            print(f"{cls.ANSI_RED}Tests failed: {e}{cls.ANSI_RESET}")

        print(
            f"({TestBase.request_count()} requests sent with an average latency of {TestBase.average_latency()} ms)"
        )

        # clean up
        if auto:
            print("Killing server...")
            server.terminate()
            server.wait()
        else:
            print("All done, you can stop the server now.")

        if not success:
            exit(1)

    @classmethod
    def free_port(cls, port):
        for proc in psutil.process_iter():
            try:
                conns = proc.connections(kind="inet")
            except psutil.AccessDenied:
                conns = []
            for conn in conns:
                if conn.laddr.port == port:
                    print(f"Stopping {proc.name()} to free the port {port}...")
                    proc.send_signal(signal.SIGTERM)
                    proc.wait()
                    break

    @classmethod
    def compver(cls, target):
        requests.packages.urllib3.disable_warnings(
            requests.packages.urllib3.exceptions.InsecureRequestWarning
        )
        server = requests.get("https://localhost:5443/api", verify=False).headers.get(
            "X-Gallium-Version"
        )

        if server is None:
            print(f"{cls.ANSI_YELLOW}Warning : unknown server version{cls.ANSI_RESET}")
            return 0

        try:
            target_major, target_minor, target_patch, *_ = [
                int(num) for num in target.split(".")[:3]
            ] + [0, 0]
            server_major, server_minor, server_patch, *_ = [
                int(num) for num in server.split(".")[:3]
            ] + [0, 0]
        except ValueError:
            print(f"{cls.ANSI_YELLOW}Warning : cannot parse versions{cls.ANSI_RESET}")
            return 0

        less = False

        if server_major == target_major:
            if server_minor == target_minor:
                less = server_patch < target_patch
            else:
                less = server_minor < target_minor
        else:
            less = server_major < target_major

        if less:
            print(f"{cls.ANSI_RED}Error : Detected v{server}")
            print(
                f"This test suite is targeting v{target} and higher, aborting.{cls.ANSI_RESET}"
            )
        else:
            print(f"OK : Detected v{server}\n")

        return less
