Feature: Basic Calculator Operations
  As a user of the calculator
  I want to perform basic arithmetic operations
  So that I can calculate mathematical results

  Background:
    Given I have a calculator

  Scenario: Adding two numbers
    Given I have entered 50 into the calculator
    And I have entered 70 into the calculator
    When I press add
    Then the result should be 120 on the screen

  Scenario: Subtracting two numbers
    Given I have entered 100 into the calculator
    And I have entered 35 into the calculator
    When I press subtract
    Then the result should be 65 on the screen

  Scenario: Multiplying two numbers
    Given I have entered 8 into the calculator
    And I have entered 7 into the calculator
    When I press multiply
    Then the result should be 56 on the screen

  Scenario: Dividing two numbers
    Given I have entered 100 into the calculator
    And I have entered 5 into the calculator
    When I press divide
    Then the result should be 20 on the screen

  Scenario Outline: Adding different numbers
    Given I have entered <first> into the calculator
    And I have entered <second> into the calculator
    When I press add
    Then the result should be <result> on the screen

    Examples:
      | first | second | result |
      | 10    | 20     | 30     |
      | 15    | 25     | 40     |
      | 0     | 0      | 0      |
      | -5    | 5      | 0      |
      | -10   | -20    | -30    |
