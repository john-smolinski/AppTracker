import React from "react";
import "../styles/menu.css";

// hard coded without functionality while I short out the layout.
export default function Menu() {
  return (
    <div className="menu-bar">
      <button className="menu-button">Search</button>
      <button className="menu-button">Add New</button>
      <button className="menu-button">Statistics</button>
    </div>
  );
}
