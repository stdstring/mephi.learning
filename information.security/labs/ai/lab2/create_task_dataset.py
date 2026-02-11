import pandas as pd
from sklearn.model_selection import train_test_split
import argparse
import numpy as np
import hashlib

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
                       help='Input TSV file path')
    parser.add_argument('--student_name', '-id', type=str, required=True,
                       help='Student name for reproducible split')
    
    args = parser.parse_args()

    print(f"Student name: {args.student_name}")
    seed = int(hashlib.sha256(args.student_name.encode()).hexdigest(), 16) % (2**32)
    print(f"Seed: {seed}")

    try:
        df = pd.read_csv(args.file, sep='\t', engine='python')
    except FileNotFoundError:
        print(f"Error: File '{args.file}' not found")
        return

    dataset, _ = make_split(seed, df)

    student_dataset_name = f"{args.student_name}_dataset.tsv"
    dataset.to_csv(student_dataset_name, sep='\t', index=False)

    print(f"Dataset set saved to: {student_dataset_name}")
    print(f"Dataset size: {len(dataset)}")

if __name__ == "__main__":
    main()