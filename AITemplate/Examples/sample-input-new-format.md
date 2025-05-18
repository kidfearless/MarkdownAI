# AI Chat Template - New Format

config:
provider: OpenAI
apikey: $OPENAI_API_KEY
model: gpt-4

system:
You are a data science consultant specializing in Python and machine learning. You provide practical advice and code examples to help data scientists solve problems.

user:
I'm working with a dataset that has missing values in several columns. What are the best approaches for handling missing data in pandas, and when should I use each approach?

assistant:
# Handling Missing Data in Pandas

Here's a comprehensive guide to dealing with missing values in pandas:

## 1. Identifying Missing Values

First, you need to identify missing values in your dataset:

```python
import pandas as pd
import numpy as np

# Check missing values
df.isna().sum()  # Count missing values per column
df.info()        # Get overall info including non-null counts
```

## 2. Main Approaches for Handling Missing Data

### A. Deletion Methods

```python
# Drop rows with any missing values
df_cleaned = df.dropna()

# Drop rows where all values are missing
df_cleaned = df.dropna(how='all')

# Drop rows if they have fewer than n non-missing values
df_cleaned = df.dropna(thresh=4)

# Drop specific columns with missing values
df_cleaned = df.dropna(axis=1)
```

**When to use**: 
- When missing data is a small percentage of your dataset
- When the missing values are random (MCAR - Missing Completely At Random)
- For quick exploratory analysis

### B. Imputation Methods

```python
# Fill with a specific value
df_filled = df.fillna(0)

# Fill with mean/median/mode
df_filled = df.fillna(df.mean())  # Mean for each column
df_filled = df.fillna(df.median())  # Median for each column
df_filled = df.fillna(df['column_name'].mode()[0])  # Mode

# Forward fill (use previous value)
df_filled = df.fillna(method='ffill')

# Backward fill (use next value)
df_filled = df.fillna(method='bfill')

# Interpolation (linear by default)
df_filled = df.interpolate()
```

**When to use**:
- Mean: For normally distributed numerical data without outliers
- Median: For numerical data with outliers
- Mode: For categorical data
- Forward/backward fill: For time series data
- Interpolation: For data with some inherent order or progression

### C. Advanced Imputation

```python
# Using scikit-learn's imputers
from sklearn.impute import SimpleImputer, KNNImputer

# Simple imputer
imputer = SimpleImputer(strategy='mean')  # or 'median', 'most_frequent', 'constant'
df_imputed = pd.DataFrame(imputer.fit_transform(df), columns=df.columns)

# KNN imputer (uses k-nearest neighbors)
imputer = KNNImputer(n_neighbors=5)
df_imputed = pd.DataFrame(imputer.fit_transform(df), columns=df.columns)
```

**When to use**:
- When relationships between features are important
- With structured, meaningful data where patterns exist
- When simple imputation methods don't capture the complexity

## 3. Best Practices

1. **Understand your data first** - The nature of your data and why values are missing should inform your approach
   
2. **Consider creating a 'missing' indicator** - Sometimes the fact that data is missing is informative:
   ```python
   df['column_missing'] = df['column'].isna().astype(int)
   ```

3. **Try multiple approaches** - Compare model performance with different imputation methods

4. **Be cautious with imputation** - Document your approach and be aware of how it might bias your analysis

5. **For sensitive applications** - Consider using multiple imputation techniques or models that handle missing values internally

Let me know if you need specific examples for your particular dataset!
