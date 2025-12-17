import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;

public class TaskManager {

    public static void main(String[] args) {
        List<String> tasks = new ArrayList<>();
        Scanner scanner = new Scanner(System.in);
        boolean running = true;

        while (running) {
            System.out.println("1. Добавить задачу");
            System.out.println("2. Удалить задачу");
            System.out.println("3. Показать задачи");
            System.out.println("4. Выйти");

            String choice = scanner.nextLine();

            switch (choice) {
                case "1":
                    System.out.println("Введите название задачи:");
                    tasks.add(scanner.nextLine());
                    break;

                case "2":
                    System.out.println("Введите номер задачи:");
                    int index = Integer.parseInt(scanner.nextLine());
                    if (index >= 0 && index < tasks.size()) {
                        tasks.remove(index);
                    }
                    break;

                case "3":
                    for (int i = 0; i < tasks.size(); i++) {
                        System.out.println(i + ": " + tasks.get(i));
                    }
                    break;

                case "4":
                    running = false;
                    break;
            }
        }
    }
}
