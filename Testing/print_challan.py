import unittest
from selenium import webdriver
from selenium.webdriver.common.by import By
import time
import os

class TestPDFDownload(unittest.TestCase):
    def setUp(self):
        with open("url.txt", "r") as file:
            self.base_url = file.read().strip()
        self.driver = webdriver.Chrome()  
        self.driver.get(self.base_url + "/Home/Login")

        # Perform login
        self.driver.find_element(By.XPATH, "//input[@id='numlId']").send_keys("NUML-S24-10154")
        self.driver.find_element(By.XPATH, "//input[@id='password']").send_keys("aA2@34567")
        self.driver.find_element(By.XPATH, "//input[@type='submit']").click()
        time.sleep(5)

    def test_pdf_download(self):
        # Find and click the button
        button = self.driver.find_element(By.CSS_SELECTOR, "button.delete-btn")
        button.click()

        # Wait for the PDF to download
        time.sleep(5)  # Adjust the sleep time based on how long it usually takes for the PDF to download

        # Check if a PDF file with "FeeChallan" in its name is downloaded
        download_dir = "C:\\Users\\PMLS\\Downloads"
        files = os.listdir(download_dir)
        pdf_files = [file for file in files if "FeeChallan" in file and file.endswith(".pdf")]

        # Assert that at least one PDF file containing "FeeChallan" in its name is downloaded
        self.assertTrue(len(pdf_files) > 0, "No PDF file containing 'FeeChallan' in its name was downloaded.")

    def tearDown(self):
        # Quit the WebDriver
        self.driver.quit()

if __name__ == "__main__":
    unittest.main()
