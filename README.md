# BDD Educational Project

An educational project demonstrating **Behavior-Driven Development (BDD)** principles using Python and Behave.

## 📚 What is BDD?

**Behavior-Driven Development (BDD)** is an agile software development methodology that encourages collaboration between developers, QA, and non-technical business participants. BDD focuses on obtaining a clear understanding of desired software behavior through discussion with stakeholders.

### Key Principles of BDD:

1. **Ubiquitous Language**: Use a common language that all stakeholders understand
2. **Living Documentation**: Tests serve as up-to-date documentation
3. **Outside-In Development**: Start with the user's perspective
4. **Executable Specifications**: Requirements that can be automatically verified

## 🎯 Project Structure

```
BDD-project/
├── features/                          # BDD feature files
│   ├── calculator.feature            # Basic calculator operations
│   ├── error_handling.feature        # Error handling scenarios
│   ├── advanced_operations.feature   # Advanced features
│   └── steps/                        # Step definitions
│       └── calculator_steps.py       # Implementation of steps
├── src/                              # Application source code
│   ├── __init__.py
│   └── calculator.py                 # Calculator implementation
├── requirements.txt                  # Python dependencies
├── behave.ini                        # Behave configuration
└── README.md                         # This file
```

## 🚀 Getting Started

### Prerequisites

- Python 3.7 or higher
- pip (Python package installer)

### Installation

1. Clone this repository:
```bash
git clone https://github.com/BohdanaSeveryn/BDD-project.git
cd BDD-project
```

2. Install dependencies:
```bash
pip install -r requirements.txt
```

Or install as a package (recommended for development):
```bash
pip install -e .
```

### Running the Tests

Execute all BDD scenarios:
```bash
behave
```

Run specific feature file:
```bash
behave features/calculator.feature
```

Run with verbose output:
```bash
behave -v
```

## 📝 Understanding Gherkin Syntax

Gherkin is the language used to write BDD scenarios. It uses a simple, human-readable syntax:

### Keywords:

- **Feature**: High-level description of a software feature
- **Scenario**: Concrete example of how the feature should behave
- **Given**: Set up the initial context
- **When**: Describe the action/event
- **Then**: Expected outcome
- **And/But**: Additional steps
- **Background**: Common steps for all scenarios in a feature
- **Scenario Outline**: Template for multiple scenarios with examples

### Example Scenario:

```gherkin
Feature: Calculator Addition
  As a user
  I want to add two numbers
  So that I can get the sum

  Scenario: Add two positive numbers
    Given I have a calculator
    And I have entered 50 into the calculator
    And I have entered 70 into the calculator
    When I press add
    Then the result should be 120 on the screen
```

## 🧪 Example Features

### 1. Basic Operations
Demonstrates simple arithmetic operations (add, subtract, multiply, divide) using BDD scenarios.

### 2. Error Handling
Shows how to test error conditions and exception handling, such as division by zero.

### 3. Advanced Operations
Illustrates more complex scenarios including state management and result storage.

## 🔍 BDD Workflow

```
1. Write Feature File (Gherkin)
   ↓
2. Run Tests (They fail - Red)
   ↓
3. Implement Step Definitions
   ↓
4. Implement Application Code
   ↓
5. Run Tests (They pass - Green)
   ↓
6. Refactor Code
   ↓
7. Repeat
```

## 💡 Learning Resources

### Key Concepts:

1. **User Stories**: BDD starts with user stories written in a standard format:
   - As a [role]
   - I want [feature]
   - So that [benefit]

2. **Three Amigos**: Collaboration between:
   - Business (defines the feature)
   - Development (implements the feature)
   - Testing (verifies the feature)

3. **Acceptance Criteria**: Clear conditions that must be met for a feature to be accepted

### Benefits of BDD:

- ✅ Better collaboration between technical and non-technical team members
- ✅ Living documentation that's always up-to-date
- ✅ Reduced ambiguity in requirements
- ✅ Easier regression testing
- ✅ Focus on user needs and business value
- ✅ Improved test coverage

## 🛠️ Extending the Project

To add new features:

1. **Create a new feature file** in the `features/` directory
2. **Write scenarios** using Gherkin syntax
3. **Run the tests** to see them fail
4. **Implement step definitions** in `features/steps/`
5. **Implement application code** in `src/`
6. **Run tests again** to verify they pass

## 📖 Additional Resources

- [Behave Documentation](https://behave.readthedocs.io/)
- [Cucumber Documentation](https://cucumber.io/docs/guides/)
- [Gherkin Reference](https://cucumber.io/docs/gherkin/reference/)
- [BDD on Wikipedia](https://en.wikipedia.org/wiki/Behavior-driven_development)

## 🤝 Contributing

This is an educational project. Feel free to:
- Add new feature scenarios
- Improve documentation
- Add more complex examples
- Create tutorials or guides

## 📄 License

This project is created for educational purposes.

## 👥 Authors

- Educational project demonstrating BDD principles

---

**Happy Testing! 🎉**

Remember: *BDD is not about testing; it's about collaboration and understanding what we're building!*