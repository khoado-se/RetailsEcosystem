import { useEffect, useState, useCallback } from "react";
import { getCategories } from "./categoryApi";

export const useCategories = () => {
  const [categories, setCategories] = useState([]);
  const [tick, setTick] = useState(0);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await getCategories();
        setCategories(res.data.items);
      } catch (err) {
        console.error(err);
      }
    };
    fetchData();
  }, [tick]);

  const fetchCategories = useCallback(() => setTick((t) => t + 1), []);

  return { categories, fetchCategories };
};
