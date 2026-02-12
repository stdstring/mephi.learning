import cvxpy as cp

if __name__ == "__main__":
    y1 = cp.Variable(nonneg=True)
    y2 = cp.Variable(nonneg=True)
    y3 = cp.Variable(nonneg=True)
    y4 = cp.Variable(nonneg=True)
    w = cp.Variable(nonneg=True)
    objective = cp.Minimize(w)
    constraints = [
        2 * y1 + 4 * y2 + 8 * y3 + 5 * y4 <= w,
        6 * y1 + 2 * y2 + 4 * y3 + 6 * y4 <= w,
        3 * y1 + 2 * y2 + 5 * y3 + 4 * y4 <= w,
        y1 + y2 + y3 + y4 == 1
    ]
    problem = cp.Problem(objective, constraints)
    problem.solve()
    print(f"Status: {problem.status}")
    print(f"Optimal value: {problem.value:.2f}")
    print(f"y1: {y1.value:.2f}")
    print(f"y2: {y2.value:.2f}")
    print(f"y3: {y3.value:.2f}")
    print(f"y4: {y4.value:.2f}")
    print("Check constraint:")
    print(f"2 * y1 + 4 * y2 + 8 * y3 + 5 * y4 = {(2 * y1 + 4 * y2 + 8 * y3 + 5 * y4).value:.2f} <= {w.value:.2f}")
    print(f"6 * y1 + 2 * y2 + 4 * y3 + 6 * y4 = {(6 * y1 + 2 * y2 + 4 * y3 + 6 * y4).value:.2f} <= {w.value:.2f}")
    print(f"3 * y1 + 2 * y2 + 5 * y3 + 4 * y4 = {(3 * y1 + 2 * y2 + 5 * y3 + 4 * y4).value:.2f} <= {w.value:.2f}")
    print(f"y1 + y2 + y3 + y4 = {(y1 + y2 + y3 + y4).value:.2f} == 1")