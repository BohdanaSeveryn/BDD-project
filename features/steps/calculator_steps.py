"""
Step definitions for calculator BDD tests.

This module implements the steps defined in the feature files,
demonstrating how Gherkin scenarios map to actual Python code.
"""

from behave import given, when, then
import sys
import os

# Add the src directory to the path so we can import the calculator
sys.path.insert(0, os.path.join(os.path.dirname(__file__), '../../src'))

from calculator import Calculator


@given('I have a calculator')
def step_impl(context):
    """Initialize a calculator instance for the test."""
    context.calculator = Calculator()
    context.numbers = []
    context.error = None


@given('I have entered {number:d} into the calculator')
def step_impl(context, number):
    """Store a number to be used in a calculation."""
    context.numbers.append(number)


@given('the calculator result is set to {value:d}')
def step_impl(context, value):
    """Set the calculator's result to a specific value."""
    context.calculator.set_result(value)


@when('I press add')
def step_impl(context):
    """Perform addition operation."""
    if len(context.numbers) >= 2:
        result = context.calculator.add(context.numbers[0], context.numbers[1])
        context.calculator.set_result(result)


@when('I press subtract')
def step_impl(context):
    """Perform subtraction operation."""
    if len(context.numbers) >= 2:
        result = context.calculator.subtract(context.numbers[0], context.numbers[1])
        context.calculator.set_result(result)


@when('I press multiply')
def step_impl(context):
    """Perform multiplication operation."""
    if len(context.numbers) >= 2:
        result = context.calculator.multiply(context.numbers[0], context.numbers[1])
        context.calculator.set_result(result)


@when('I press divide')
def step_impl(context):
    """Perform division operation, handling potential errors."""
    if len(context.numbers) >= 2:
        try:
            result = context.calculator.divide(context.numbers[0], context.numbers[1])
            context.calculator.set_result(result)
        except ValueError as e:
            context.error = str(e)


@when('I press power')
def step_impl(context):
    """Perform power operation."""
    if len(context.numbers) >= 2:
        result = context.calculator.power(context.numbers[0], context.numbers[1])
        context.calculator.set_result(result)


@when('I press clear')
def step_impl(context):
    """Clear the calculator result."""
    context.calculator.clear()


@when('I store the result')
def step_impl(context):
    """Store the current result for later verification."""
    context.stored_result = context.calculator.get_result()


@then('the result should be {expected:d} on the screen')
def step_impl(context, expected):
    """Verify that the calculator result matches the expected value."""
    actual = context.calculator.get_result()
    assert actual == expected, f"Expected {expected}, but got {actual}"


@then('I should see an error message "{message}"')
def step_impl(context, message):
    """Verify that the expected error message was raised."""
    assert context.error is not None, "Expected an error but none was raised"
    assert context.error == message, f"Expected error '{message}', but got '{context.error}'"


@then('no error should be raised')
def step_impl(context):
    """Verify that no error occurred."""
    assert context.error is None, f"Unexpected error occurred: {context.error}"


@then('the stored result should be {expected:d}')
def step_impl(context, expected):
    """Verify that the stored result matches the expected value."""
    assert hasattr(context, 'stored_result'), "No result was stored"
    assert context.stored_result == expected, f"Expected stored result {expected}, but got {context.stored_result}"
