import unittest
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import TimeoutException
import time

class NumlPayLoginTest(unittest.TestCase):
    def setUp(self):
        with open("url.txt", "r") as file:
            self.base_url = file.read().strip()
        self.driver = webdriver.Chrome()  
        self.driver.get(self.base_url + "/Home/Login")

    def test_login_with_valid_credentials(self):
        self.driver.find_element(By.XPATH, "//input[@id='numlId']").send_keys("NUML-F20-10154")
        self.driver.find_element(By.XPATH, "//input[@id='password']").send_keys("aA2@34567")
        self.driver.find_element(By.XPATH, "//input[@type='submit']").click()

        time.sleep(5)  

        self.assertIn("/main/dashboard", self.driver.current_url.lower())

    def test_login_with_invalid_credentials(self):
        username = "NUML-F20-10186"
        password = "11223344Tt^"
        
        self.driver.find_element(By.XPATH, "//input[@id='numlId']").send_keys(username)
        self.driver.find_element(By.XPATH, "//input[@id='password']").send_keys(password)
        self.driver.find_element(By.XPATH, "//input[@type='submit']").click()
        
        time.sleep(2)  
        error_message = self.driver.find_element(By.XPATH, "//div[@id='customAlert']").text.strip()
        first_line = error_message.split('\n')[0].strip('"')
        self.assertEqual(first_line, "Wrong Credentails!!")

    def tearDown(self):
        self.driver.quit()


if __name__ == "__main__":
    unittest.main()
