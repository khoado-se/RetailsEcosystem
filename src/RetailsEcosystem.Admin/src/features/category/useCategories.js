import { useEffect, useState } from "react";
import { getCategories } from "./categoryApi";

export const useCategories = () => {
  const [categories, setCategories] = useState([]);

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
  }, []);

  return { categories };
};