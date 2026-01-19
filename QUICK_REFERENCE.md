# BDD Quick Reference

Quick reference guide for Behavior-Driven Development with Behave.

## 📋 Gherkin Keywords

| Keyword | Purpose | Example |
|---------|---------|---------|
| `Feature:` | Describe a feature | `Feature: User Login` |
| `Scenario:` | Describe a test case | `Scenario: Valid login` |
| `Given` | Set up context | `Given I am on the login page` |
| `When` | Perform action | `When I enter valid credentials` |
| `Then` | Assert outcome | `Then I should see my dashboard` |
| `And` | Add more steps | `And I should see a welcome message` |
| `But` | Add contrasting step | `But I should not see login form` |
| `Background:` | Common setup | `Background: Given I have...` |
| `Scenario Outline:` | Template with examples | See below |
| `Examples:` | Data table for outline | See below |

## 🎯 Basic Gherkin Patterns

### Simple Scenario
```gherkin
Scenario: Add two numbers
  Given I have a calculator
  When I add 2 and 3
  Then the result should be 5
```

### Multiple Steps
```gherkin
Scenario: Complete user registration
  Given I am on the registration page
  And I have valid user details
  When I submit the registration form
  Then I should see a success message
  And I should receive a confirmation email
  And I should be redirected to the login page
```

### Scenario Outline with Examples
```gherkin
Scenario Outline: Validate different calculations
  Given I have a calculator
  When I calculate <first> <operation> <second>
  Then the result should be <result>

  Examples:
    | first | operation | second | result |
    | 2     | plus      | 3      | 5      |
    | 10    | minus     | 4      | 6      |
    | 5     | times     | 3      | 15     |
```

### Background
```gherkin
Feature: Shopping Cart
  
  Background:
    Given I am a logged-in customer
    And my cart is empty
  
  Scenario: Add item to cart
    When I add a product to my cart
    Then my cart should contain 1 item
```

## 🔧 Step Definition Patterns (Python)

### Basic Steps
```python
from behave import given, when, then

@given('I have a calculator')
def step_impl(context):
    context.calculator = Calculator()

@when('I add {a:d} and {b:d}')
def step_impl(context, a, b):
    context.result = context.calculator.add(a, b)

@then('the result should be {expected:d}')
def step_impl(context, expected):
    assert context.result == expected
```

### Parameter Types
```python
# Integer
@given('I have {count:d} items')
def step_impl(context, count):
    pass

# Float
@when('the price is {price:f}')
def step_impl(context, price):
    pass

# String (with quotes)
@then('I should see "{message}"')
def step_impl(context, message):
    pass

# Word (without quotes)
@given('the status is {status:w}')
def step_impl(context, status):
    pass
```

### Data Tables
```gherkin
Given I have the following users:
  | name  | email              | role  |
  | Alice | alice@example.com  | admin |
  | Bob   | bob@example.com    | user  |
```

```python
@given('I have the following users')
def step_impl(context):
    for row in context.table:
        name = row['name']
        email = row['email']
        role = row['role']
        # Create user...
```

### Multiline Text
```gherkin
Given a blog post with content:
  """
  This is a long blog post.
  It has multiple paragraphs.
  
  And blank lines too.
  """
```

```python
@given('a blog post with content')
def step_impl(context):
    content = context.text
    # Use content...
```

## 🎨 Common BDD Patterns

### Resource Setup/Teardown
```python
@given('I have a database connection')
def step_impl(context):
    context.db = Database.connect()
    # Cleanup happens in environment.py after_scenario
```

### Error Handling
```python
@when('I try to {action}')
def step_impl(context, action):
    try:
        # Attempt action
        context.result = perform_action(action)
    except Exception as e:
        context.error = e

@then('I should see an error')
def step_impl(context):
    assert context.error is not None
```

### State Verification
```python
@then('the {entity} should be {state}')
def step_impl(context, entity, state):
    actual_state = get_state(entity)
    assert actual_state == state
```

## 🚀 Behave Commands

### Basic Commands
```bash
# Run all tests
behave

# Run specific feature
behave features/calculator.feature

# Run with tags
behave --tags=@smoke

# Verbose output
behave -v

# No capture (see print statements)
behave --no-capture

# Stop on first failure
behave --stop

# Dry run (check syntax)
behave --dry-run
```

### Formatting Options
```bash
# Pretty format (default)
behave --format=pretty

# Progress bar
behave --format=progress

# JSON output
behave --format=json -o report.json

# Multiple formats
behave --format=pretty --format=json -o report.json
```

### Tags
```gherkin
@smoke @priority-high
Scenario: Critical user login
  Given I am on the login page
  When I log in with valid credentials
  Then I should see my dashboard
```

```bash
# Run smoke tests
behave --tags=@smoke

# Exclude slow tests
behave --tags=~@slow

# Multiple tag conditions
behave --tags=@smoke --tags=@priority-high
```

## 📁 Project Structure

```
project/
├── features/
│   ├── environment.py          # Hooks (setup/teardown)
│   ├── feature1.feature        # Feature file
│   ├── feature2.feature
│   └── steps/
│       ├── __init__.py
│       ├── common_steps.py     # Reusable steps
│       └── specific_steps.py   # Feature-specific steps
├── src/
│   └── application_code.py     # Application code
├── behave.ini                  # Behave configuration
└── requirements.txt
```

## ⚙️ Configuration (behave.ini)

```ini
[behave]
# Output format
default_format = pretty
show_timings = true

# Output capture
stdout_capture = false
stderr_capture = false
log_capture = false

# Colors
color = true

# Tags
default_tags = -@skip -@wip

# Paths
paths = features/
```

## 🔍 Context Object

The `context` object is shared across steps in a scenario:

```python
# Store values
context.user = User("John")
context.result = 42
context.items = []

# Common attributes
context.text          # Multiline text from scenario
context.table         # Data table from scenario
context.failed        # Whether scenario has failed
context.feature       # Current feature
context.scenario      # Current scenario
```

## 🎭 Environment Hooks

```python
# features/environment.py

def before_all(context):
    """Run once before all tests"""
    # Setup database, start services, etc.
    pass

def after_all(context):
    """Run once after all tests"""
    # Cleanup
    pass

def before_feature(context, feature):
    """Run before each feature"""
    pass

def after_feature(context, feature):
    """Run after each feature"""
    pass

def before_scenario(context, scenario):
    """Run before each scenario"""
    # Reset state
    pass

def after_scenario(context, scenario):
    """Run after each scenario"""
    # Cleanup
    if scenario.status == "failed":
        # Take screenshot, save logs, etc.
        pass

def before_step(context, step):
    """Run before each step"""
    pass

def after_step(context, step):
    """Run after each step"""
    pass

def before_tag(context, tag):
    """Run for specific tags"""
    if tag == "require_database":
        context.db = Database.connect()

def after_tag(context, tag):
    """Cleanup for specific tags"""
    if tag == "require_database":
        context.db.close()
```

## 💡 Best Practices

### DO ✅
- Use business language, not technical terms
- Focus on behavior, not implementation
- Keep scenarios independent
- Use declarative style (what, not how)
- Make scenarios readable by non-technical stakeholders
- Reuse step definitions
- Use meaningful examples

### DON'T ❌
- Couple scenarios together
- Test implementation details
- Use technical jargon in scenarios
- Make scenarios too long
- Duplicate step definitions
- Test multiple features in one scenario
- Skip the Given-When-Then structure

## 📊 Example: Complete Feature

```gherkin
@shopping @high-priority
Feature: Shopping Cart
  As a customer
  I want to manage items in my shopping cart
  So that I can purchase products

  Background:
    Given I am logged in as a customer
    And my cart is empty

  @smoke
  Scenario: Add item to empty cart
    Given I am viewing product "Laptop"
    When I add the product to my cart
    Then my cart should contain 1 item
    And the cart total should be $999

  Scenario: Remove item from cart
    Given I have "Laptop" in my cart
    When I remove "Laptop" from my cart
    Then my cart should be empty

  Scenario Outline: Add multiple items
    Given I am viewing product "<product>"
    When I add <quantity> items to my cart
    Then my cart should contain <quantity> items

    Examples:
      | product | quantity |
      | Laptop  | 1        |
      | Mouse   | 2        |
      | Keyboard| 1        |

  @error-handling
  Scenario: Handle out-of-stock item
    Given product "Limited Edition" is out of stock
    When I try to add "Limited Edition" to my cart
    Then I should see error "Product is out of stock"
    And my cart should be empty
```

## 🔗 Resources

- **Behave Docs**: https://behave.readthedocs.io/
- **Gherkin Reference**: https://cucumber.io/docs/gherkin/
- **BDD Guide**: See `BDD_GUIDE.md` in this project
- **Examples**: See `features/` directory

---

**Quick Tip**: Start with simple scenarios and gradually add complexity. BDD is about collaboration and understanding, not just testing! 🎯
