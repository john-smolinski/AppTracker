import React from "react";
import Link from "next/link";
import styles from "./Menu.module.css";

// hard coded without functionality while I short out the layout.
export default function Menu() {
  return (
    <nav className={styles["menu-bar"]}>
      <Link href="/" className={styles["menu-button"]}>
        Home
      </Link>
      <Link href="/applications" className={styles["menu-button"]}>
        Applications
      </Link>
      <Link href="/add" className={styles["menu-button"]}>
        Add New
      </Link>
      <Link href="/statistics" className={styles["menu-button"]}>
        Charts
      </Link>
    </nav>
  );
}
