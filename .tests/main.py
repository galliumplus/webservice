import unittest
import urllib3
import requests
import decimal

from utils.launcher import Launcher

from tests import *

if __name__ == "__main__":
    decimal.DefaultContext.prec = 2
    Launcher.launch("1.2.0")
