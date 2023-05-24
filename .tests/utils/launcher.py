import os, sys
import subprocess
import http.client
import time
import unittest
import psutil, signal

from utils.test_base import TestBase


class Launcher:
    ANSI_RED_BOLD = "\x1b[1;31m"
    ANSI_RESET = "\x1b[0m"

    @classmethod
    def launch(cls):
        if "--help" in sys.argv:
            unittest.main()

        auto = "--manual" not in sys.argv
        if not auto:
            sys.argv.remove("--manual")

        if "--long-timeout" in sys.argv:
            sys.argv.remove("--long-timeout")
            timeout = 60
        else:
            timeout = 10

        success = False

        # free port
        if auto:
            try:
                cls.free_port(5443)
            except:
                pass

        # start test server
        if auto:
            print("Starting server in TEST mode...")
            server = subprocess.Popen(
                ["dotnet", "run", "--project", "Service", "-c", "Test"]
            )
        else:
            print("Waiting for server to start...")

        # wait for server
        ping_client = http.client.HTTPConnection("localhost", 5443)

        for n in range(timeout):
            try:
                ping_client.connect()
                print("Server is up !")
                ping_client.close()
                break
            except ConnectionRefusedError:
                if server.poll():
                    print(f"{cls.ANSI_RED_BOLD}Failed to start server.{cls.ANSI_RESET}")
                    sys.exit(1)
                else:
                    print(f"Server not up yet, retrying soon... ({n+1}/{timeout})")
                    time.sleep(2)
        else:
            print(f"{cls.ANSI_RED_BOLD}Timed out.{cls.ANSI_RESET}")
            sys.exit(1)

        # run tests
        try:
            result = unittest.main(exit=False).result
            success = result.wasSuccessful()
        except Exception as e:
            print(f"{cls.ANSI_RED_BOLD}Tests failed: {e}{cls.ANSI_RESET}")

        print(f"({TestBase.request_count()} requests sent)")

        # clean up
        if auto:
            print("Killing server...")
            server.kill()
        else:
            print("All done, you can stop the server now.")

        if not success:
            exit(1)

    @classmethod
    def free_port(cls, port):
        for proc in psutil.process_iter():
            for conns in proc.connections(kind="inet"):
                if conns.laddr.port == port:
                    print(f"Stopping {proc.name()} to free the port {port}...")
                    proc.send_signal(signal.SIGTERM)
                    break
