"""
Behave environment configuration.

This file contains hooks that run before and after certain events
during BDD test execution. It's used for test setup and teardown.
"""


def before_all(context):
    """
    Run before the entire test suite.
    
    Use this for:
    - Setting up database connections
    - Starting services
    - Global test configuration
    """
    print("\n=== Starting BDD Test Suite ===")
    print("Educational BDD Project - Calculator Tests\n")


def after_all(context):
    """
    Run after the entire test suite.
    
    Use this for:
    - Closing database connections
    - Stopping services
    - Cleanup operations
    """
    print("\n=== BDD Test Suite Completed ===\n")


def before_feature(context, feature):
    """
    Run before each feature file.
    
    Args:
        context: The test context
        feature: The feature being tested
    """
    print(f"\n--- Testing Feature: {feature.name} ---")


def after_feature(context, feature):
    """
    Run after each feature file.
    
    Args:
        context: The test context
        feature: The feature that was tested
    """
    pass


def before_scenario(context, scenario):
    """
    Run before each scenario.
    
    Args:
        context: The test context
        scenario: The scenario being tested
    """
    # Reset any shared state if needed
    context.error = None


def after_scenario(context, scenario):
    """
    Run after each scenario.
    
    Args:
        context: The test context
        scenario: The scenario that was tested
    """
    # Cleanup after scenario if needed
    pass


def before_step(context, step):
    """
    Run before each step.
    
    Args:
        context: The test context
        step: The step being executed
    """
    pass


def after_step(context, step):
    """
    Run after each step.
    
    Args:
        context: The test context
        step: The step that was executed
    """
    pass
