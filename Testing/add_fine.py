import unittest
from selenium import webdriver
from selenium.webdriver.common.by import By
import time

class AddFineTest(unittest.TestCase):
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

        # Navigate to Add Fine page
        self.driver.get(self.base_url + "/Fine/addFine")

    def test_add_fine_in_valid(self):
        # Perform actions to add a fine
        # Select Session
        session_dropdown = self.driver.find_element(By.ID, "session")
        session_dropdown.send_keys("Spring 2024")

        # Select Fine For
        fine_for_dropdown = self.driver.find_element(By.ID, "fine_for")
        fine_for_dropdown.send_keys("Tuition Fee")

        # Enter Fine Til 10 Days of Due Date
        fine_10_days_input = self.driver.find_element(By.ID, "fine_after_10_days")
        fine_10_days_input.send_keys("1000")

        # Enter Fine Til 30 Days of Due Date
        fine_30_days_input = self.driver.find_element(By.ID, "fine_after_30_days")
        fine_30_days_input.send_keys("2000")

        # Enter Fine After 60 Days of Due Date
        fine_60_days_input = self.driver.find_element(By.ID, "fine_after_60_days")
        fine_60_days_input.send_keys("5000")

        # Click Add Fine button
        add_fine_button = self.driver.find_element(By.XPATH, "//input[@value='Add Fine']")
        add_fine_button.click()

        # Add assertions based on the expected behavior after submitting the form
        # Wait for success message
        time.sleep(5)
        error_message = self.driver.find_element(By.XPATH, "//div[@id='customAlert']").text.strip()
        first_line = error_message.split('\n')[0].strip('"')
        self.assertEqual(first_line, "Fine Already Exists for this Session.")
        
    def test_add_fine_valid(self):
        # Perform actions to add a fine
        # Select Session
        session_dropdown = self.driver.find_element(By.ID, "session")
        session_dropdown.send_keys("Spring 2024")

        # Select Fine For
        fine_for_dropdown = self.driver.find_element(By.ID, "fine_for")
        fine_for_dropdown.send_keys("Tuition Fee")

        # Enter Fine Til 10 Days of Due Date
        fine_10_days_input = self.driver.find_element(By.ID, "fine_after_10_days")
        fine_10_days_input.send_keys("1000")

        # Enter Fine Til 30 Days of Due Date
        fine_30_days_input = self.driver.find_element(By.ID, "fine_after_30_days")
        fine_30_days_input.send_keys("2000")

        # Enter Fine After 60 Days of Due Date
        fine_60_days_input = self.driver.find_element(By.ID, "fine_after_60_days")
        fine_60_days_input.send_keys("5000")

        # Click Add Fine button
        add_fine_button = self.driver.find_element(By.XPATH, "//input[@value='Add Fine']")
        add_fine_button.click()

        # Add assertions based on the expected behavior after submitting the form
        # Wait for success message
        time.sleep(5)
        error_message = self.driver.find_element(By.XPATH, "//div[@id='customAlert']").text.strip()
        first_line = error_message.split('\n')[0].strip('"')
        self.assertEqual(first_line, "Fine Created Successfully!")

    def tearDown(self):
        # Quit the WebDriver
        self.driver.quit()

if __name__ == "__main__":
    suite = unittest.TestLoader().loadTestsFromTestCase(AddFineTest)
    tests = [suite._tests.pop(1), suite._tests.pop(0)]  # Swap test order
    suite.addTests(tests)
    unittest.TextTestRunner().run(suite)