import cvxpy as cp

if __name__ == "__main__":
    x1 = cp.Variable(integer=True, nonneg=True)
    x2 = cp.Variable(integer=True, nonneg=True)
    objective = cp.Maximize(40 * x1 + 60 * x2)
    constraints = [
        3 * x1 + 2 * x2 <= 120,
        2 * x1 + 5 * x2 <= 150
    ]
    problem = cp.Problem(objective, constraints)
    problem.solve()
    print(f"Status: {problem.status}")
    print(f"Optimal value (max profit): {problem.value:.2f}")
    print(f"x1 (Product 1): {x1.value:.2f}")
    print(f"x2 (Product 2): {x2.value:.2f}")
    print("Check constraint:")
    print(f"3 * x1 + 2 * x2 = {(3 * x1 + 2 * x2).value} <= 120")
    print(f"2 * x1 + 5 * x2 = {(2 * x1 + 5 * x2).value} <= 150")
