Feature: Division by Zero Error Handling
  As a user of the calculator
  I want to be protected from dividing by zero
  So that I get appropriate error messages instead of crashes

  Background:
    Given I have a calculator

  Scenario: Attempting to divide by zero
    Given I have entered 10 into the calculator
    And I have entered 0 into the calculator
    When I press divide
    Then I should see an error message "Cannot divide by zero"

  Scenario: Division by non-zero works correctly
    Given I have entered 20 into the calculator
    And I have entered 4 into the calculator
    When I press divide
    Then the result should be 5 on the screen
    And no error should be raised
