import unittest
from selenium import webdriver
from selenium.webdriver.common.by import By
import time

class AddInstallmentTest(unittest.TestCase):
    def setUp(self):
        # Load the WebDriver and open the URL
        with open("url.txt", "r") as file:
            self.base_url = file.read().strip()
        self.driver = webdriver.Chrome()  
        self.driver.get(self.base_url + "/Home/loginAdmin")

        # Perform login
        self.driver.find_element(By.XPATH, "//input[@id='email_id']").send_keys("hamza@numl.edu.pk")
        self.driver.find_element(By.XPATH, "//input[@id='password']").send_keys("aA2@345678")
        self.driver.find_element(By.XPATH, "//input[@type='submit']").click()
        time.sleep(5)

        # Navigate to Add Installment page
        self.driver.get(self.base_url + "/InstallmentManagement/addInstallment")

    def test_add_installment_invalid(self):
        # Perform actions to add an installment
        # Select Mode of Installments
        mode_dropdown = self.driver.find_element(By.ID, "mode")
        mode_dropdown.send_keys("No Installments")

        # Select Fee For
        fee_for_dropdown = self.driver.find_element(By.ID, "fee_for")
        fee_for_dropdown.send_keys("Hostel Fee")

        # Click Add Installment button
        add_installment_button = self.driver.find_element(By.XPATH, "//input[@value='Add Installment']")
        add_installment_button.click()

        # Add assertions based on the expected behavior after submitting the form
        # Wait for error message
        time.sleep(5)
        error_message = self.driver.find_element(By.XPATH, "//div[@id='customAlert']").text.strip()
        first_line = error_message.split('\n')[0].strip('"')
        self.assertEqual(first_line, "Installment Mode Already Exists.")

    def test_add_installment_valid(self):
        # Perform actions to add an installment
        # Select Mode of Installments
        mode_dropdown = self.driver.find_element(By.ID, "mode")
        mode_dropdown.send_keys("No Installments")

        # Select Fee For
        fee_for_dropdown = self.driver.find_element(By.ID, "fee_for")
        fee_for_dropdown.send_keys("Hostel Fee")

        # Click Add Installment button
        add_installment_button = self.driver.find_element(By.XPATH, "//input[@value='Add Installment']")
        add_installment_button.click()

        # Add assertions based on the expected behavior after submitting the form
        # Wait for success message
        time.sleep(5)
        success_message = self.driver.find_element(By.ID, "customAlert").text.strip()
        self.assertEqual(success_message, "Installment Created Successfully!")

    def tearDown(self):
        # Quit the WebDriver
        self.driver.quit()

if __name__ == "__main__":
    suite = unittest.TestLoader().loadTestsFromTestCase(AddInstallmentTest)
    tests = [suite._tests.pop(1), suite._tests.pop(0)]  # Swap test order
    suite.addTests(tests)
    unittest.TextTestRunner().run(suite)
