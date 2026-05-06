// Returns an array of error message strings. Empty array means the form is valid.
export function validateProductForm(form) {
  const errors = [];
  if (!form.name?.trim())                        errors.push("Product name is required.");
  if (!form.price || Number(form.price) <= 0)    errors.push("Price must be greater than 0.");
  if (!form.categoryId)                          errors.push("Please select a category.");
  return errors;
}
