import pandas as pd
from sklearn.model_selection import train_test_split
import argparse
import numpy as np
import hashlib

def add_noise(seed, data, noise_level=0.03):
    np.random.seed(seed)
    data_noisy = data.copy()
    numeric_columns = data_noisy.select_dtypes(include=[np.number]).columns
    for column in numeric_columns:
        std_dev = data_noisy[column].std()
        
        # Пропускаем столбцы с нулевым стандартным отклонением (константные)
        if std_dev == 0:
            continue

        noise = np.random.normal(0, noise_level * std_dev, size=len(data_noisy))
        data_noisy[column] = data_noisy[column] + noise
    
    return data_noisy

def make_split(seed, df, label_column='label', test_size=0.3):
    train, test = train_test_split(
        df, 
        test_size=test_size, 
        random_state=seed,
        stratify=df[label_column]
    )
    
    return train, test

def main():
    parser = argparse.ArgumentParser(description='Create student dataset')
    parser.add_argument('--file', '-f', type=str, required=True,
                       help='Input CSV file path')
    parser.add_argument('--student_name', '-id', type=str, required=True,
                       help='Student name for reproducible split')
    
    args = parser.parse_args()

    print(f"Student name: {args.student_name}")
    seed = int(hashlib.sha256(args.student_name.encode()).hexdigest(), 16) % (2**32)
    print(f"Seed: {seed}")

    try:
        df = pd.read_csv(args.file)
    except FileNotFoundError:
        print(f"Error: File '{args.file}' not found")
        return

    dataset, _ = make_split(seed, df)

    dataset = add_noise(seed, dataset)

    student_dataset_name = f"{args.student_name}_dataset.csv"
    dataset.to_csv(student_dataset_name, index=False)

    print(f"Dataset set saved to: {student_dataset_name}")
    print(f"Dataset size: {len(dataset)}")

if __name__ == "__main__":
    main()