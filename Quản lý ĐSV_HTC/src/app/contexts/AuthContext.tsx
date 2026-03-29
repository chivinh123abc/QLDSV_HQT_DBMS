import { createContext, useContext, ReactNode, useState, useEffect } from "react";

type Role = "PGV" | "KHOA" | "SV";

interface User {
  name: string;
  role: Role;
  department: string;
  studentId?: string;
}

interface AuthContextType {
  currentRole: Role | null;
  currentUser: User | null;
  login: (username: string, password: string) => boolean;
  logout: () => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

// ============================================================================
// TEST ACCOUNTS DATABASE
// ============================================================================
interface TestAccount {
  username: string;
  password: string;
  user: User;
}

const testAccounts: TestAccount[] = [
  {
    username: "sv",
    password: "sv",
    user: {
      name: "Nguyễn Văn Minh",
      role: "SV",
      department: "Khoa Công Nghệ Thông Tin",
      studentId: "2051052001",
    },
  },
  {
    username: "pgv",
    password: "pgv",
    user: {
      name: "Nguyễn Văn A",
      role: "PGV",
      department: "Phòng Giáo Vụ",
    },
  },
  {
    username: "khoa",
    password: "khoa",
    user: {
      name: "Trần Thị B",
      role: "KHOA",
      department: "Khoa Công Nghệ Thông Tin",
    },
  },
];

export function AuthProvider({ children }: { children: ReactNode }) {
  const [currentUser, setCurrentUser] = useState<User | null>(null);
  const [currentRole, setCurrentRole] = useState<Role | null>(null);

  // Load saved session from localStorage on mount
  useEffect(() => {
    const savedUser = localStorage.getItem("currentUser");
    if (savedUser) {
      const user = JSON.parse(savedUser);
      setCurrentUser(user);
      setCurrentRole(user.role);
    }
  }, []);

  const login = (username: string, password: string): boolean => {
    const account = testAccounts.find(
      (acc) => acc.username === username && acc.password === password
    );

    if (account) {
      setCurrentUser(account.user);
      setCurrentRole(account.user.role);
      localStorage.setItem("currentUser", JSON.stringify(account.user));
      return true;
    }

    return false;
  };

  const logout = () => {
    setCurrentUser(null);
    setCurrentRole(null);
    localStorage.removeItem("currentUser");
  };

  const isAuthenticated = currentUser !== null;

  return (
    <AuthContext.Provider
      value={{ currentRole, currentUser, login, logout, isAuthenticated }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}