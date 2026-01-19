"""
Quick Start Example - Interactive Calculator Demo

This script demonstrates how to use the calculator and shows
how the BDD scenarios translate to real code usage.
"""

import sys
import os

# NOTE: This manual path manipulation is for educational purposes only.
# In production, install the package properly: pip install -e .
# This allows direct imports: from src.calculator import Calculator
project_root = os.path.abspath(os.path.dirname(__file__))
if project_root not in sys.path:
    sys.path.insert(0, project_root)

from src.calculator import Calculator


def demo_basic_operations():
    """Demonstrate basic calculator operations."""
    print("=" * 50)
    print("BASIC CALCULATOR OPERATIONS DEMO")
    print("=" * 50)
    
    calc = Calculator()
    
    # Addition (corresponds to calculator.feature scenario)
    print("\n📊 Addition Example:")
    print("   Scenario: Adding 50 and 70")
    result = calc.add(50, 70)
    print(f"   Result: 50 + 70 = {result}")
    print(f"   ✓ Test passed: {result == 120}")
    
    # Subtraction
    print("\n📊 Subtraction Example:")
    print("   Scenario: Subtracting 35 from 100")
    result = calc.subtract(100, 35)
    print(f"   Result: 100 - 35 = {result}")
    print(f"   ✓ Test passed: {result == 65}")
    
    # Multiplication
    print("\n📊 Multiplication Example:")
    print("   Scenario: Multiplying 8 and 7")
    result = calc.multiply(8, 7)
    print(f"   Result: 8 × 7 = {result}")
    print(f"   ✓ Test passed: {result == 56}")
    
    # Division
    print("\n📊 Division Example:")
    print("   Scenario: Dividing 100 by 5")
    result = calc.divide(100, 5)
    print(f"   Result: 100 ÷ 5 = {result}")
    print(f"   ✓ Test passed: {result == 20}")


def demo_error_handling():
    """Demonstrate error handling."""
    print("\n" + "=" * 50)
    print("ERROR HANDLING DEMO")
    print("=" * 50)
    
    calc = Calculator()
    
    print("\n⚠️  Division by Zero Example:")
    print("   Scenario: Attempting to divide 10 by 0")
    try:
        result = calc.divide(10, 0)
        print(f"   Result: {result}")
    except ValueError as e:
        print(f"   ✓ Error caught: {e}")
        print(f"   ✓ Calculator handled error gracefully")


def demo_advanced_operations():
    """Demonstrate advanced operations."""
    print("\n" + "=" * 50)
    print("ADVANCED OPERATIONS DEMO")
    print("=" * 50)
    
    calc = Calculator()
    
    # Power operation
    print("\n🔢 Power Example:")
    print("   Scenario: 2 raised to power of 8")
    result = calc.power(2, 8)
    print(f"   Result: 2^8 = {result}")
    print(f"   ✓ Test passed: {result == 256}")
    
    # State management
    print("\n💾 State Management Example:")
    print("   Scenario: Storing and retrieving results")
    calc.set_result(100)
    print(f"   Set result to: 100")
    stored = calc.get_result()
    print(f"   Retrieved result: {stored}")
    print(f"   ✓ Test passed: {stored == 100}")
    
    # Clear operation
    print("\n🔄 Clear Example:")
    print("   Scenario: Clearing the calculator")
    calc.clear()
    result = calc.get_result()
    print(f"   Result after clear: {result}")
    print(f"   ✓ Test passed: {result == 0}")


def interactive_mode():
    """Run calculator in interactive mode."""
    print("\n" + "=" * 50)
    print("INTERACTIVE CALCULATOR MODE")
    print("=" * 50)
    print("\nCommands:")
    print("  add <a> <b>       - Add two numbers")
    print("  subtract <a> <b>  - Subtract b from a")
    print("  multiply <a> <b>  - Multiply two numbers")
    print("  divide <a> <b>    - Divide a by b")
    print("  power <base> <exp> - Raise base to power of exp")
    print("  clear             - Clear the calculator")
    print("  quit              - Exit interactive mode")
    print()
    
    calc = Calculator()
    
    while True:
        try:
            command = input("calculator> ").strip().lower()
            
            if not command:
                continue
                
            if command == "quit":
                print("Goodbye! 👋")
                break
                
            if command == "clear":
                calc.clear()
                print(f"Calculator cleared. Result: {calc.get_result()}")
                continue
            
            parts = command.split()
            if len(parts) < 3:
                print("Invalid command. Use: <operation> <a> <b>")
                continue
                
            operation = parts[0]
            a = float(parts[1])
            b = float(parts[2])
            
            if operation == "add":
                result = calc.add(a, b)
            elif operation == "subtract":
                result = calc.subtract(a, b)
            elif operation == "multiply":
                result = calc.multiply(a, b)
            elif operation == "divide":
                result = calc.divide(a, b)
            elif operation == "power":
                result = calc.power(a, b)
            else:
                print(f"Unknown operation: {operation}")
                continue
                
            calc.set_result(result)
            print(f"Result: {result}")
            
        except ValueError as e:
            print(f"Error: {e}")
        except Exception as e:
            print(f"Unexpected error: {e}")


def main():
    """Main entry point."""
    print("\n" + "=" * 50)
    print("BDD EDUCATIONAL PROJECT - CALCULATOR DEMO")
    print("=" * 50)
    print("\nThis demo shows how BDD scenarios translate to real code.")
    print("Each demo corresponds to scenarios in the feature files.")
    
    # Run demonstrations
    demo_basic_operations()
    demo_error_handling()
    demo_advanced_operations()
    
    # Ask if user wants interactive mode
    print("\n" + "=" * 50)
    response = input("\nWould you like to try interactive mode? (y/n): ").strip().lower()
    if response == 'y':
        interactive_mode()
    else:
        print("\nTo run the BDD tests, use: behave")
        print("To read more about BDD, see: BDD_GUIDE.md")
        print("\nThank you for exploring BDD! 🎉")


if __name__ == "__main__":
    main()
