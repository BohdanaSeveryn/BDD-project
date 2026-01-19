"""
Simple Calculator module for BDD demonstration.

This module provides basic arithmetic operations to demonstrate
Behavior-Driven Development principles.
"""


class Calculator:
    """A simple calculator class with basic arithmetic operations."""
    
    def __init__(self):
        """Initialize the calculator with a result of 0."""
        self.result = 0
    
    def add(self, a, b):
        """
        Add two numbers and return the result.
        
        Args:
            a: First number
            b: Second number
            
        Returns:
            The sum of a and b
        """
        return a + b
    
    def subtract(self, a, b):
        """
        Subtract b from a and return the result.
        
        Args:
            a: First number
            b: Second number
            
        Returns:
            The difference of a and b
        """
        return a - b
    
    def multiply(self, a, b):
        """
        Multiply two numbers and return the result.
        
        Args:
            a: First number
            b: Second number
            
        Returns:
            The product of a and b
        """
        return a * b
    
    def divide(self, a, b):
        """
        Divide a by b and return the result.
        
        Args:
            a: Numerator
            b: Denominator
            
        Returns:
            The quotient of a and b
            
        Raises:
            ValueError: If b is zero
        """
        if b == 0:
            raise ValueError("Cannot divide by zero")
        return a / b
    
    def power(self, base, exponent):
        """
        Raise base to the power of exponent.
        
        Args:
            base: The base number
            exponent: The exponent
            
        Returns:
            base raised to the power of exponent
        """
        return base ** exponent
    
    def set_result(self, value):
        """
        Set the calculator's current result.
        
        Args:
            value: The value to set as the result
        """
        self.result = value
    
    def get_result(self):
        """
        Get the calculator's current result.
        
        Returns:
            The current result
        """
        return self.result
    
    def clear(self):
        """Reset the calculator result to 0."""
        self.result = 0
