import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchApplications } from "../redux/applicationSlice";
import "../styles/summary.css";

export default function Summary() {
  const dispatch = useDispatch();

  const { items, loading, error, isServerError } = useSelector(
    (state) => state.applications
  );

  useEffect(() => {
    dispatch(fetchApplications());
  }, [dispatch]);

  // logging for development
  //useEffect(() => {
  //  console.log(items.length);
  //  items.forEach((x) => {
  //    console.log(x);
  //    //console.log(sourceCounts);
  //  });
  //});

  const sourceCounts = items.reduce((accur, itm) => {
    const sourceName = itm.source.name;
    accur[sourceName] = (accur[sourceName] || 0) + 1;
    return accur;
  }, {});

  if (loading)
    return (
      <div className="summary">
        <p>Loading...</p>
      </div>
    );
  if (isServerError)
    return (
      <div>
        <h3 className="summary">
          Server error occurred. Please try again later.
        </h3>
      </div>
    );
  if (error)
    return (
      <div className="summary">
        <h1>Error:</h1> <p> {error.message}</p>
      </div>
    );
  return (
    <div>
      <h1>Summary View</h1>
      <div className="summary">
        <h3>{items.length} Total Applications submitted </h3>
        <div>
          <ul>
            {Object.entries(sourceCounts).map(([key, value]) => (
              <li key={key}>
                {key} : {value}
              </li>
            ))}
          </ul>
        </div>
      </div>
    </div>
  );
}
