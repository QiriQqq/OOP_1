# Мутационный анализ

Пять мутаций соответствуют списку из варианта. Скрипт `RunMutations.ps1` вносит каждую мутацию отдельно, запускает тесты и восстанавливает исходный файл в блоке `finally`. Итоговые исходники после работы скрипта остаются без мутаций.

| № | Мутация | Тест, который должен обнаружить её |
|---:|---|---|
| 1 | В проверке часов `hours > 60` заменяется на `hours > 61` | `Hours_outside_allowed_range_are_rejected` при значении 61 |
| 2 | Удаляется проверка `salary < MIN_SALARY` | `Salary_below_minimum_is_rejected` |
| 3 | В расчёте надбавки за стаж деление на 100 заменяется умножением на 100 | `Seniority_bonus_uses_injected_time_and_completed_years` |
| 4 | Средняя выплата делится на длину массива вместо числа занятых мест | `Average_payment_uses_only_occupied_employee_slots` |
| 5 | Процентная премия всегда возвращает 0 | `Salaried_employee_payment_includes_percentage_bonus` и `Percentage_bonus_is_not_ignored` |

Запуск в PowerShell из корня архива:

```powershell
.\RunMutations.ps1
```

Скрипт ожидает, что каждая мутация приводит к ненулевому коду завершения `dotnet test`. Результаты каждого прогона выводятся в консоль.
