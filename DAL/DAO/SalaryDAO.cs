using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DAO
{
    public class SalaryDAO : EmployeeContext
    {
        public static List<Months> GetMonths()
        {
            return db.Months.ToList();
        }

        public static void AddSalary(Salary salary)
        {
            // Crear un NUEVO contexto para esta operación
            using (var freshContext = new EmployeeDataClassDataContext())
            {
                try
                {
                    // Verificar si ya existe en la base de datos (no en el contexto)
                    bool exists = freshContext.Salary.Any(s =>
                        s.EmployeeID == salary.EmployeeID &&
                        s.Year == salary.Year &&
                        s.MonthID == salary.MonthID);

                    if (exists)
                    {
                        // Obtener el registro existente
                        var existing = freshContext.Salary.First(s =>
                            s.EmployeeID == salary.EmployeeID &&
                            s.Year == salary.Year &&
                            s.MonthID == salary.MonthID);

                        // Actualizar solo el monto
                        existing.Amount = salary.Amount;
                    }
                    else
                    {
                        // Crear un NUEVO objeto Salary para insertar
                        var newSalary = new Salary
                        {
                            EmployeeID = salary.EmployeeID,
                            Amount = salary.Amount,
                            Year = salary.Year,
                            MonthID = salary.MonthID
                            // No asignar ID (se autogenerará)
                        };

                        freshContext.Salary.InsertOnSubmit(newSalary);
                    }

                    freshContext.SubmitChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al guardar el salario: " + ex.Message);
                }
            }
        }

        public static List<SalaryDetailDTO> GetSalaries()
        {
            List<SalaryDetailDTO> salaryList = new List<SalaryDetailDTO>();
            var list = (from s in db.Salary
                        join e in db.Employee on s.EmployeeID equals e.ID
                        join m in db.Months on s.MonthID equals m.ID
                        select new
                        {
                            userNo = e.UserNo,
                            name = e.Name,
                            surname = e.Surname,
                            employeeID = s.EmployeeID,
                            amount = s.Amount,
                            year = s.Year,
                            monthName = m.MonthName,
                            monthID = s.MonthID,
                            salaryID = s.ID,
                            departmentID = e.DepartmentID,
                            positionID = e.PositionID
                        }).OrderBy(x => x.year).ToList();
            foreach (var item in list)
            {
                SalaryDetailDTO dto = new SalaryDetailDTO();
                dto.UserNo = item.userNo;
                dto.Name = item.name;
                dto.EmployeeID = item.employeeID;
                dto.SalaryAmount = item.amount;
                dto.SalaryYear = item.year;
                dto.MonthName = item.monthName;
                dto.MonthID = item.monthID;
                dto.SalaryID = item.salaryID;
                dto.DepartmentID = item.departmentID;
                dto.PositionID = item.positionID;
                dto.OldSalary = item.amount;
                salaryList.Add(dto);
            }
            return salaryList;
        }
    }
}
