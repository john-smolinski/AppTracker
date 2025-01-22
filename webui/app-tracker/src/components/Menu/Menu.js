import React from "react";
import { Link } from "react-router-dom";
import "./Menu.css";

// hard coded without functionality while I short out the layout.
export default function Menu() {
  return (
    <div className="menu-bar">
      <Link to="/" className="menu-button">
        Home
      </Link>
      <Link to="/search" className="menu-button">
        Search
      </Link>
      <Link to="/add" className="menu-button">
        Add New
      </Link>
      <Link to="/statistics" className="menu-button">
        Statistics
      </Link>
    </div>
  );
}
