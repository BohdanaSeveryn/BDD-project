Feature: Advanced Calculator Operations
  As an advanced user
  I want to perform more complex calculations
  So that I can solve advanced mathematical problems

  Background:
    Given I have a calculator

  Scenario: Calculating power
    Given I have entered 2 into the calculator
    And I have entered 8 into the calculator
    When I press power
    Then the result should be 256 on the screen

  Scenario: Clearing the calculator
    Given I have entered 50 into the calculator
    And the calculator result is set to 100
    When I press clear
    Then the result should be 0 on the screen

  Scenario: Storing and retrieving results
    Given I have entered 10 into the calculator
    And I have entered 5 into the calculator
    When I press add
    Then the result should be 15 on the screen
    When I store the result
    Then the stored result should be 15
